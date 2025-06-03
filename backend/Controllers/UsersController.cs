using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartNameplate.Api.Entities;
using SmartNameplate.Api.Services;
using System.Security.Claims;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔐 要求所有端點都需要認證
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService, 
            ISecurityService securityService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _securityService = securityService;
            _logger = logger;
        }

        /// <summary>
        /// 🔐 取得所有用戶 (僅管理員)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            _securityService.LogSecurityEvent(SecurityEventType.DataAccess, 
                "Admin accessed all users list",
                new { AdminId = currentUserId, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
            
            var users = await _userService.GetAllUsersAsync();
            
            // 🛡️ 不返回密碼雜湊值
            var safeUsers = users.Select(u => new
            {
                u.Id,
                u.UserName,
                u.Role,
                u.CreatedAt
                // 故意排除 PasswordHash
            }).ToList();
            
            return Ok(safeUsers);
        }

        /// <summary>
        /// 🔐 根據用戶名取得用戶 (自己或管理員)
        /// </summary>
        [HttpGet("{username}")]
        public async Task<ActionResult<object>> GetUserByUsername(string username)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            // 🛡️ 輸入清理
            var sanitizedUsername = _securityService.SanitizeHtml(username);
            
            // 🔐 權限檢查：只能查看自己的資料，或管理員可以查看所有人
            if (currentRole != "admin" && currentUsername != sanitizedUsername)
            {
                _securityService.LogSecurityEvent(SecurityEventType.UnauthorizedAccess, 
                    $"User {currentUsername} attempted to access {sanitizedUsername}'s profile",
                    new { CurrentUserId = currentUserId, TargetUsername = sanitizedUsername, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                
                return Forbid();
            }
            
            var user = await _userService.GetUserByUsernameAsync(sanitizedUsername);
            if (user == null)
            {
                return NotFound(new { message = $"用戶 '{sanitizedUsername}' 不存在" });
            }
            
            _securityService.LogSecurityEvent(SecurityEventType.DataAccess, 
                $"User profile accessed: {user.UserName}",
                new { AccessedBy = currentUserId, TargetUserId = user.Id, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
            
            // 🛡️ 不返回密碼雜湊值
            var safeUser = new
            {
                user.Id,
                user.UserName,
                user.Role,
                user.CreatedAt
                // 故意排除 PasswordHash
            };
            
            return Ok(safeUser);
        }

        /// <summary>
        /// 🔐 創建新用戶 (僅管理員)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<object>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                // 🛡️ 輸入驗證和清理
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "使用者名稱和密碼不能為空" });
                }

                var sanitizedUsername = _securityService.SanitizeHtml(request.Username);
                var sanitizedRole = _securityService.SanitizeHtml(request.Role);

                // 🔐 驗證用戶名格式
                if (!_securityService.ValidateUsername(sanitizedUsername))
                {
                    return BadRequest(new { message = "使用者名稱格式不正確（3-30個字符，只允許字母、數字、底線、連字符，且不能以數字開頭）" });
                }

                // 🔐 驗證密碼強度
                if (!_securityService.ValidatePasswordStrength(request.Password))
                {
                    return BadRequest(new { message = "密碼強度不足（至少8個字符，包含大小寫字母、數字和特殊字符）" });
                }

                // 🔐 驗證角色
                var allowedRoles = new[] { "admin", "user" };
                if (!allowedRoles.Contains(sanitizedRole.ToLower()))
                {
                    return BadRequest(new { message = "無效的角色，只允許 'admin' 或 'user'" });
                }
                
                var user = await _userService.CreateUserAsync(sanitizedUsername, request.Password, sanitizedRole);
                
                _securityService.LogSecurityEvent(SecurityEventType.DataAccess, 
                    $"New user created by admin: {user.UserName}",
                    new { AdminId = currentUserId, NewUserId = user.Id, NewUserRole = user.Role, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                
                // 🛡️ 返回安全的用戶資料
                var safeUser = new
                {
                    user.Id,
                    user.UserName,
                    user.Role,
                    user.CreatedAt
                };
                
                return CreatedAtAction(nameof(GetUserByUsername), new { username = user.UserName }, safeUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建用戶時發生錯誤");
                return StatusCode(500, new { message = "創建用戶時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 🔐 更新用戶密碼 (自己或管理員)
        /// </summary>
        [HttpPut("{username}/password")]
        public async Task<ActionResult<object>> UpdateUserPassword(string username, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
                var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // 🛡️ 輸入清理
                var sanitizedUsername = _securityService.SanitizeHtml(username);
                
                // 🔐 權限檢查：只能修改自己的密碼，或管理員可以修改所有人
                if (currentRole != "admin" && currentUsername != sanitizedUsername)
                {
                    _securityService.LogSecurityEvent(SecurityEventType.UnauthorizedAccess, 
                        $"User {currentUsername} attempted to change {sanitizedUsername}'s password",
                        new { CurrentUserId = currentUserId, TargetUsername = sanitizedUsername, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                    
                    return Forbid();
                }
                
                // 🔐 驗證新密碼強度
                if (!_securityService.ValidatePasswordStrength(request.NewPassword))
                {
                    return BadRequest(new { message = "新密碼強度不足（至少8個字符，包含大小寫字母、數字和特殊字符）" });
                }
                
                var user = await _userService.UpdateUserPasswordAsync(sanitizedUsername, request.NewPassword);
                
                _securityService.LogSecurityEvent(SecurityEventType.PasswordChange, 
                    $"Password updated for user: {user.UserName}",
                    new { UpdatedBy = currentUserId, TargetUserId = user.Id, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                
                // 🛡️ 返回安全的用戶資料
                var safeUser = new
                {
                    user.Id,
                    user.UserName,
                    user.Role,
                    user.CreatedAt,
                    message = "密碼更新成功"
                };
                
                return Ok(safeUser);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新密碼時發生錯誤");
                return StatusCode(500, new { message = "更新密碼時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 🔐 批量創建用戶 (僅管理員)
        /// </summary>
        [HttpPost("batch")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<object>> CreateUsers([FromBody] List<CreateUserRequest> requests)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var createdUsers = new List<object>();
            var errors = new List<string>();

            if (requests == null || !requests.Any())
            {
                return BadRequest(new { message = "請求列表不能為空" });
            }

            // 🔐 限制批量操作數量
            if (requests.Count > 50)
            {
                return BadRequest(new { message = "單次批量操作不能超過50個用戶" });
            }

            _securityService.LogSecurityEvent(SecurityEventType.DataAccess, 
                $"Admin started batch user creation: {requests.Count} users",
                new { AdminId = currentUserId, RequestCount = requests.Count, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });

            foreach (var request in requests)
            {
                try
                {
                    // 🛡️ 輸入驗證和清理
                    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                    {
                        errors.Add($"{request.Username}: 使用者名稱和密碼不能為空");
                        continue;
                    }

                    var sanitizedUsername = _securityService.SanitizeHtml(request.Username);
                    var sanitizedRole = _securityService.SanitizeHtml(request.Role);

                    // 🔐 驗證用戶名格式
                    if (!_securityService.ValidateUsername(sanitizedUsername))
                    {
                        errors.Add($"{sanitizedUsername}: 使用者名稱格式不正確");
                        continue;
                    }

                    // 🔐 驗證密碼強度
                    if (!_securityService.ValidatePasswordStrength(request.Password))
                    {
                        errors.Add($"{sanitizedUsername}: 密碼強度不足");
                        continue;
                    }

                    // 🔐 驗證角色
                    var allowedRoles = new[] { "admin", "user" };
                    if (!allowedRoles.Contains(sanitizedRole.ToLower()))
                    {
                        errors.Add($"{sanitizedUsername}: 無效的角色");
                        continue;
                    }
                    
                    var user = await _userService.CreateUserAsync(sanitizedUsername, request.Password, sanitizedRole);
                    
                    // 🛡️ 返回安全的用戶資料
                    createdUsers.Add(new
                    {
                        user.Id,
                        user.UserName,
                        user.Role,
                        user.CreatedAt
                    });
                }
                catch (InvalidOperationException ex)
                {
                    errors.Add($"{request.Username}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "批量創建用戶時發生錯誤: {Username}", request.Username);
                    errors.Add($"{request.Username}: 創建時發生內部錯誤");
                }
            }

            _securityService.LogSecurityEvent(SecurityEventType.DataAccess, 
                $"Admin completed batch user creation",
                new { 
                    AdminId = currentUserId, 
                    TotalRequested = requests.Count,
                    TotalCreated = createdUsers.Count,
                    TotalErrors = errors.Count,
                    RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() 
                });

            return Ok(new 
            { 
                CreatedUsers = createdUsers, 
                Errors = errors,
                TotalCreated = createdUsers.Count,
                TotalErrors = errors.Count
            });
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }

    public class UpdatePasswordRequest
    {
        public string NewPassword { get; set; } = null!;
    }
} 