using Microsoft.AspNetCore.Mvc;
using SmartNameplate.Api.Services;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ISecurityService securityService, ILogger<ContactController> logger)
        {
            _securityService = securityService;
            _logger = logger;
        }

        /// <summary>
        /// 🛡️ 發送聯絡訊息 (受 CSRF 保護)
        /// </summary>
        [HttpPost("send")]
        [ValidateAntiForgeryToken]
        public ActionResult SendMessage([FromBody] ContactMessageRequest request)
        {
            try
            {
                // 🛡️ 輸入驗證和清理
                if (string.IsNullOrWhiteSpace(request.Name) || 
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest(new { message = "姓名、電子郵件和訊息內容為必填項目" });
                }

                var sanitizedName = _securityService.SanitizeHtml(request.Name);
                var sanitizedEmail = _securityService.SanitizeHtml(request.Email);
                var sanitizedCompany = _securityService.SanitizeHtml(request.Company ?? "");
                var sanitizedPhone = _securityService.SanitizeHtml(request.Phone ?? "");
                var sanitizedMessage = _securityService.SanitizeHtml(request.Message);

                // 驗證電子郵件格式
                if (!IsValidEmail(sanitizedEmail))
                {
                    return BadRequest(new { message = "電子郵件格式不正確" });
                }

                _securityService.LogSecurityEvent(SecurityEventType.DataAccess,
                    "Contact form submitted",
                    new { 
                        Name = sanitizedName,
                        Email = sanitizedEmail,
                        Company = sanitizedCompany,
                        RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString()
                    });

                // 這裡可以添加實際的郵件發送邏輯
                // 例如：使用 SendGrid、SMTP 等服務發送郵件

                _logger.LogInformation("聯絡訊息已收到 - 姓名: {Name}, 電子郵件: {Email}", 
                    sanitizedName, sanitizedEmail);

                return Ok(new { 
                    message = "感謝您的聯絡！我們會盡快回覆您。",
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "處理聯絡訊息時發生錯誤");
                return StatusCode(500, new { message = "處理聯絡訊息時發生內部錯誤" });
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    public class ContactMessageRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Company { get; set; }
        public string? Phone { get; set; }
        public string Message { get; set; } = null!;
    }
} 