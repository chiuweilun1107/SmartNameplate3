using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace SmartNameplate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AntiforgeryController : ControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<AntiforgeryController> _logger;

        public AntiforgeryController(IAntiforgery antiforgery, ILogger<AntiforgeryController> logger)
        {
            _antiforgery = antiforgery;
            _logger = logger;
        }

        /// <summary>
        /// 🛡️ 獲取 CSRF Token
        /// </summary>
        [HttpGet("token")]
        public IActionResult GetToken()
        {
            try
            {
                var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
                
                // 設置 CSRF Cookie
                Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
                {
                    HttpOnly = false, // 必須允許 JavaScript 讀取
                    SameSite = SameSiteMode.Strict,
                    Secure = Request.IsHttps,
                    Path = "/"
                });

                _logger.LogInformation("CSRF token generated and sent to client");

                return Ok(new { 
                    message = "CSRF token generated successfully",
                    tokenHeaderName = "X-XSRF-TOKEN"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating CSRF token");
                return StatusCode(500, new { message = "Error generating CSRF token" });
            }
        }

        /// <summary>
        /// 🛡️ 驗證 CSRF Token（測試用）
        /// </summary>
        [HttpPost("validate")]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateToken()
        {
            try
            {
                _logger.LogInformation("CSRF token validation successful");
                return Ok(new { message = "CSRF token is valid" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CSRF token validation failed");
                return BadRequest(new { message = "CSRF token validation failed" });
            }
        }
    }
} 