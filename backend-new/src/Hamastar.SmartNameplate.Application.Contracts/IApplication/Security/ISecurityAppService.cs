//-----
// <copyright file="ISecurityAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Security
{
    /// <summary>
    /// 安全 App Service 介面
    /// </summary>
    public interface ISecurityAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 驗證：使用者登入
        /// </summary>
        /// <param name="loginRequest"> 登入請求 </param>
        /// <returns> 登入結果 </returns>
        Task<BusinessLogicResponse<LoginResponse>> Login(LoginRequest loginRequest);

        /// <summary>
        /// 登出：使用者
        /// </summary>
        /// <param name="logoutRequest"> 登出請求 </param>
        /// <returns> 登出結果 </returns>
        Task<BusinessLogicResponse<LogoutResponse>> Logout(LogoutRequest logoutRequest);

        /// <summary>
        /// 刷新：JWT Token
        /// </summary>
        /// <param name="refreshRequest"> 刷新請求 </param>
        /// <returns> 新 Token </returns>
        Task<BusinessLogicResponse<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest refreshRequest);

        /// <summary>
        /// 驗證：JWT Token
        /// </summary>
        /// <param name="validateRequest"> 驗證請求 </param>
        /// <returns> 驗證結果 </returns>
        Task<BusinessLogicResponse<ValidateTokenResponse>> ValidateToken(ValidateTokenRequest validateRequest);

        #endregion Methods
    }
} 