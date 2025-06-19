//-----
// <copyright file="SecurityRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend.Security;
using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using Hamastar.SmartNameplate.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Repositories.Security
{
    /// <summary>
    /// 安全服務儲存庫 - 參考原始 SecurityService 和 JwtService
    /// </summary>
    public class SecurityRepository : EfCoreRepository<SmartNameplateDbContext, Entities.User, Guid>, ISecurityRepository
    {
        #region Fields

        /// <summary>
        /// SettingProvider
        /// </summary>
        private readonly IConfiguration _appConfiguration;

        /// <summary>
        /// UnitOfWorkManager
        /// </summary>
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger log = Log.ForContext<SecurityRepository>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// HTML 清理器
        /// </summary>
        private readonly HtmlSanitizer _htmlSanitizer;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityRepository" /> class
        /// </summary>
        /// <param name="appConfiguration"> SettingProvider </param>
        /// <param name="contextProvider"> dbContext </param>
        /// <param name="unitOfWorkManager"> Unit of Work Manager </param>
        /// <param name="currentUser"> 目前登入的使用者 </param>
        public SecurityRepository(IConfiguration appConfiguration,
            IDbContextProvider<SmartNameplateDbContext> contextProvider,
            IUnitOfWorkManager unitOfWorkManager,
            ICurrentUser currentUser) : base(contextProvider)
        {
            _appConfiguration = appConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _currentUser = currentUser;

            // 配置 HTML 清理器 - 參考原始 SecurityService
            _htmlSanitizer = new HtmlSanitizer();
            _htmlSanitizer.AllowedTags.Clear();
            _htmlSanitizer.AllowedTags.Add("p");
            _htmlSanitizer.AllowedTags.Add("br");
            _htmlSanitizer.AllowedTags.Add("strong");
            _htmlSanitizer.AllowedTags.Add("em");
            _htmlSanitizer.AllowedAttributes.Clear();
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// HTML 清理 - 參考原始 SecurityService.SanitizeHtml
        /// </summary>
        /// <param name="input"> 輸入內容 </param>
        /// <returns> 清理後的內容 </returns>
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
                log.Error(ex, "Error sanitizing HTML input");
                return string.Empty;
            }
        }

        /// <summary>
        /// 驗證密碼強度 - 參考原始 SecurityService.ValidatePasswordStrength
        /// </summary>
        /// <param name="password"> 密碼 </param>
        /// <returns> 是否符合強度要求 </returns>
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
                log.Warning("Weak password attempted: Length={Length}, HasUpper={HasUpper}, HasLower={HasLower}, HasDigit={HasDigit}, HasSpecial={HasSpecial}",
                    password.Length, hasUpperCase, hasLowerCase, hasDigit, hasSpecialChar);
            }

            return isStrong;
        }

        /// <summary>
        /// 驗證用戶名格式 - 參考原始 SecurityService.ValidateUsername
        /// </summary>
        /// <param name="username"> 用戶名 </param>
        /// <returns> 是否符合格式要求 </returns>
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

        /// <summary>
        /// 使用者驗證 - 參考原始 SecurityService.AuthenticateUser
        /// </summary>
        /// <param name="request"> 登入請求 </param>
        /// <returns> 驗證結果 </returns>
        public async Task<LoginResponse> AuthenticateAsync(LoginRequest request)
        {
            var dbContext = await GetDbContextAsync();
            
            // 查找使用者
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == request.Username);
            
            if (user == null)
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "使用者不存在"
                };
            }

            // 驗證密碼 (這裡應該使用適當的密碼雜湊驗證)
            if (user.PasswordHash != request.Password) // TODO: 實作密碼雜湊驗證
            {
                return new LoginResponse
                {
                    IsSuccess = false,
                    Message = "密碼錯誤"
                };
            }

            // 生成 JWT Token
            var token = await GenerateJwtTokenAsync(user.Id, user.UserName, "");

            return new LoginResponse
            {
                IsSuccess = true,
                Message = "登入成功",
                Token = token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = ""
            };
        }

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        /// <param name="userId"> 使用者 ID </param>
        /// <param name="username"> 使用者名稱 </param>
        /// <param name="email"> 電子郵件 </param>
        /// <returns> JWT Token </returns>
        public async Task<string> GenerateJwtTokenAsync(Guid userId, string username, string email)
        {
            // TODO: 實作 JWT Token 生成邏輯
            // 這裡應該使用適當的 JWT 庫和設定
            
            var tokenData = new
            {
                UserId = userId,
                Username = username,
                Email = email,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            // 暫時回傳一個模擬的 token
            var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(
                System.Text.Json.JsonSerializer.Serialize(tokenData)));

            return await Task.FromResult(token);
        }

        /// <summary>
        /// 驗證 JWT Token
        /// </summary>
        /// <param name="token"> JWT Token </param>
        /// <returns> 驗證結果 </returns>
        public async Task<ValidateTokenResponse> ValidateJwtTokenAsync(string token)
        {
            try
            {
                // TODO: 實作 JWT Token 驗證邏輯
                // 這裡應該使用適當的 JWT 庫進行驗證
                
                var tokenBytes = Convert.FromBase64String(token);
                var tokenJson = System.Text.Encoding.UTF8.GetString(tokenBytes);
                var tokenData = System.Text.Json.JsonSerializer.Deserialize<dynamic>(tokenJson);

                return new ValidateTokenResponse
                {
                    IsValid = true,
                    Message = "Token 有效"
                };
            }
            catch
            {
                return new ValidateTokenResponse
                {
                    IsValid = false,
                    Message = "Token 無效"
                };
            }
        }

        /// <summary>
        /// 刷新 JWT Token
        /// </summary>
        /// <param name="refreshToken"> 刷新 Token </param>
        /// <returns> 新的 Token </returns>
        public async Task<RefreshTokenResponse> RefreshJwtTokenAsync(string refreshToken)
        {
            try
            {
                // TODO: 實作 JWT Token 刷新邏輯
                // 這裡應該驗證 refresh token 並生成新的 access token
                
                return new RefreshTokenResponse
                {
                    Success = true,
                    Message = "Token 刷新成功",
                    NewToken = await GenerateJwtTokenAsync(Guid.NewGuid(), "user", "user@example.com")
                };
            }
            catch
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Token 刷新失敗"
                };
            }
        }

        /// <summary>
        /// 記錄安全事件 - 參考原始 SecurityService.LogSecurityEvent
        /// </summary>
        /// <param name="eventType"> 事件類型 </param>
        /// <param name="message"> 訊息 </param>
        /// <param name="details"> 詳細資訊 </param>
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
                    log.Warning(logMessage, args.ToArray());
                    break;
                case SecurityEventType.AccountLockout:
                    log.Error(logMessage, args.ToArray());
                    break;
                default:
                    log.Information(logMessage, args.ToArray());
                    break;
            }
        }

        #endregion Public Methods

        #region Private Methods

        // 私有輔助方法

        #endregion Private Methods
    }

    /// <summary>
    /// 安全事件類型 - 參考原始 SecurityService.SecurityEventType
    /// </summary>
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

    /// <summary>
    /// Token 驗證結果
    /// </summary>
    public class TokenValidationResult
    {
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 電子郵件
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
} 