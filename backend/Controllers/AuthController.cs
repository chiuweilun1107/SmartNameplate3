using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartNameplate.Api.Services;
using System.Security.Claims;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ISecurityService _securityService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService, 
            IJwtService jwtService,
            ISecurityService securityService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _jwtService = jwtService;
            _securityService = securityService;
            _logger = logger;
        }

        /// <summary>
        /// 🔐 安全用戶登入
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // 🛡️ 輸入驗證和清理
                if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _securityService.LogSecurityEvent(SecurityEventType.LoginFailure, 
                        "Empty username or password", 
                        new { RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                    
                    return BadRequest(new { message = "使用者名稱和密碼不能為空" });
                }

                var sanitizedUsername = _securityService.SanitizeHtml(request.Username);
                
                _securityService.LogSecurityEvent(SecurityEventType.LoginAttempt, 
                    $"Login attempt for user: {sanitizedUsername}",
                    new { RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });

                var user = await _userService.GetUserByUsernameAsync(sanitizedUsername);
                if (user == null)
                {
                    _securityService.LogSecurityEvent(SecurityEventType.LoginFailure, 
                        $"User not found: {sanitizedUsername}",
                        new { RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                    
                    return BadRequest(new { message = "用戶名或密碼錯誤" });
                }

                var isValidPassword = await _userService.ValidatePasswordAsync(request.Password, user.PasswordHash);
                if (!isValidPassword)
                {
                    _securityService.LogSecurityEvent(SecurityEventType.LoginFailure, 
                        $"Invalid password for user: {sanitizedUsername}",
                        new { UserId = user.Id, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                    
                    return BadRequest(new { message = "用戶名或密碼錯誤" });
                }

                // 🔐 生成 JWT Token
                var token = _jwtService.GenerateToken(user);

                _securityService.LogSecurityEvent(SecurityEventType.LoginAttempt, 
                    $"Successful login for user: {user.UserName}",
                    new { UserId = user.Id, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });

                var response = new LoginResponse
                {
                    Success = true,
                    Message = "登入成功",
                    User = new UserInfo
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Role = user.Role
                    },
                    Token = token,
                    ExpiresIn = 3600 // 1 hour in seconds
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登入過程中發生錯誤");
                _securityService.LogSecurityEvent(SecurityEventType.SuspiciousActivity, 
                    "Login error occurred", 
                    new { Error = ex.Message, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });
                
                return StatusCode(500, new { message = "登入過程中發生內部錯誤" });
            }
        }

        /// <summary>
        /// 🔐 檢查登入狀態
        /// </summary>
        [HttpGet("status")]
        [Authorize]
        public ActionResult<object> GetStatus()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { 
                isAuthenticated = true,
                user = new
                {
                    id = userId,
                    username = username,
                    role = role
                }
            });
        }

        /// <summary>
        /// 🔐 安全登出
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            _securityService.LogSecurityEvent(SecurityEventType.LoginAttempt, 
                $"User logged out: {userId}",
                new { UserId = userId, RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString() });

            return Ok(new { message = "登出成功" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public UserInfo? User { get; set; }
        public string? Token { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
} 