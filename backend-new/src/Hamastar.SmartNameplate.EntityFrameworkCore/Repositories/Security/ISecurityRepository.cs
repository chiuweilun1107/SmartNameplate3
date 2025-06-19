//-----
// <copyright file="ISecurityRepository.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Hamastar.SmartNameplate.Repositories.Security
{
    /// <summary>
    /// 安全服務儲存庫介面
    /// </summary>
    public interface ISecurityRepository : IRepository<Entities.User, Guid>
    {
        #region Methods

        /// <summary>
        /// HTML 清理
        /// </summary>
        /// <param name="input"> 輸入字串 </param>
        /// <returns> 清理後的字串 </returns>
        string SanitizeHtml(string input);

        /// <summary>
        /// 驗證密碼強度
        /// </summary>
        /// <param name="password"> 密碼 </param>
        /// <returns> 是否符合強度要求 </returns>
        bool ValidatePasswordStrength(string password);

        /// <summary>
        /// 驗證使用者名稱
        /// </summary>
        /// <param name="username"> 使用者名稱 </param>
        /// <returns> 是否有效 </returns>
        bool ValidateUsername(string username);

        /// <summary>
        /// 使用者認證
        /// </summary>
        /// <param name="request"> 登入請求 </param>
        /// <returns> 認證結果 </returns>
        Task<LoginResponse> AuthenticateAsync(LoginRequest request);

        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        /// <param name="userId"> 使用者 ID </param>
        /// <param name="username"> 使用者名稱 </param>
        /// <param name="email"> 電子郵件 </param>
        /// <returns> JWT Token </returns>
        Task<string> GenerateJwtTokenAsync(Guid userId, string username, string email);

        /// <summary>
        /// 驗證 JWT Token
        /// </summary>
        /// <param name="token"> JWT Token </param>
        /// <returns> 驗證結果 </returns>
        Task<ValidateTokenResponse> ValidateJwtTokenAsync(string token);

        /// <summary>
        /// 刷新 JWT Token
        /// </summary>
        /// <param name="refreshToken"> 刷新 Token </param>
        /// <returns> 新的 Token </returns>
        Task<RefreshTokenResponse> RefreshJwtTokenAsync(string refreshToken);

        #endregion Methods
    }
} 