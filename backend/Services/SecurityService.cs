using Ganss.Xss;
using System.Text.RegularExpressions;

namespace SmartNameplate.Api.Services
{
    public interface ISecurityService
    {
        /// <summary>
        /// HTML 清理
        /// </summary>
        string SanitizeHtml(string input);
        
        /// <summary>
        /// 驗證密碼強度
        /// </summary>
        bool ValidatePasswordStrength(string password);
        
        /// <summary>
        /// 驗證用戶名格式
        /// </summary>
        bool ValidateUsername(string username);
        
        /// <summary>
        /// 記錄安全事件
        /// </summary>
        void LogSecurityEvent(SecurityEventType eventType, string message, object? details = null);
    }

    public enum SecurityEventType
    {
        LoginAttempt,
        LoginFailure,
        PasswordChange,
        AccountLockout,
        SuspiciousActivity,
        UnauthorizedAccess,
        DataAccess,
        PrivilegeEscalation
    }

    public class SecurityService : ISecurityService
    {
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly ILogger<SecurityService> _logger;

        public SecurityService(ILogger<SecurityService> logger)
        {
            _logger = logger;
            
            // 配置 HTML 清理器
            _htmlSanitizer = new HtmlSanitizer();
            
            // 只允許安全的 HTML 標籤
            _htmlSanitizer.AllowedTags.Clear();
            _htmlSanitizer.AllowedTags.Add("p");
            _htmlSanitizer.AllowedTags.Add("br");
            _htmlSanitizer.AllowedTags.Add("strong");
            _htmlSanitizer.AllowedTags.Add("em");
            
            // 移除所有屬性
            _htmlSanitizer.AllowedAttributes.Clear();
        }

        public string SanitizeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            try
            {
                var sanitized = _htmlSanitizer.Sanitize(input);
                
                // 記錄如果有內容被清理
                if (sanitized != input)
                {
                    LogSecurityEvent(SecurityEventType.SuspiciousActivity, 
                        "HTML content sanitized", 
                        new { Original = input, Sanitized = sanitized });
                }
                
                return sanitized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sanitizing HTML input");
                return string.Empty;
            }
        }

        public bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            // 密碼強度要求：
            // - 至少 8 個字符
            // - 包含大寫字母
            // - 包含小寫字母
            // - 包含數字
            // - 包含特殊字符
            
            var hasMinLength = password.Length >= 8;
            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasDigit = Regex.IsMatch(password, @"\d");
            var hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>?]");

            var isStrong = hasMinLength && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;

            if (!isStrong)
            {
                _logger.LogWarning("Weak password attempted: Length={Length}, HasUpper={HasUpper}, HasLower={HasLower}, HasDigit={HasDigit}, HasSpecial={HasSpecial}",
                    password.Length, hasUpperCase, hasLowerCase, hasDigit, hasSpecialChar);
            }

            return isStrong;
        }

        public bool ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            // 用戶名規則：
            // - 3-30 個字符
            // - 只允許字母、數字、底線、連字符
            // - 不能以數字開頭
            
            if (username.Length < 3 || username.Length > 30)
                return false;

            var pattern = @"^[a-zA-Z][a-zA-Z0-9_-]*$";
            var isValid = Regex.IsMatch(username, pattern);

            if (!isValid)
            {
                LogSecurityEvent(SecurityEventType.SuspiciousActivity, 
                    "Invalid username format attempted", 
                    new { Username = username });
            }

            return isValid;
        }

        public void LogSecurityEvent(SecurityEventType eventType, string message, object? details = null)
        {
            var logMessage = "SECURITY_EVENT: {EventType} - {Message}";
            var args = new List<object> { eventType, message };

            if (details != null)
            {
                logMessage += " - Details: {@Details}";
                args.Add(details);
            }

            logMessage += " - Timestamp: {Timestamp}";
            args.Add(DateTime.UtcNow);

            switch (eventType)
            {
                case SecurityEventType.UnauthorizedAccess:
                case SecurityEventType.PrivilegeEscalation:
                case SecurityEventType.SuspiciousActivity:
                    _logger.LogWarning(logMessage, args.ToArray());
                    break;
                case SecurityEventType.AccountLockout:
                    _logger.LogError(logMessage, args.ToArray());
                    break;
                default:
                    _logger.LogInformation(logMessage, args.ToArray());
                    break;
            }
        }
    }
} 