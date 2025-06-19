//-----
// <copyright file="SecurityAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Security;
using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using Hamastar.SmartNameplate.IApplication.Security;
using Hamastar.SmartNameplate.IApplication.AuditTrails;
using Hamastar.SmartNameplate.Permissions;
using Hamastar.SmartNameplate.Repositories.Security;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Security
{
    /// <summary>
    /// 安全 App
    /// </summary>
    public class SecurityAppService : ApplicationService, ISecurityAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<SecurityAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 安全儲存庫
        /// </summary>
        private readonly ISecurityRepository _securityRepository;

        /// <summary>
        /// 審計軌跡資料 APP
        /// </summary>
        private readonly IAuditTrailService _auditTrailService;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="securityRepository"> 安全儲存庫 </param>
        /// <param name="auditTrailService"> 審計軌跡資料 APP </param>
        public SecurityAppService(
            ICurrentUser currentUser,
            ISecurityRepository securityRepository,
            IAuditTrailService auditTrailService)
        {
            _currentUser = currentUser;
            _securityRepository = securityRepository;
            _auditTrailService = auditTrailService;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 驗證：使用者登入
        /// </summary>
        /// <param name="loginRequest"> 登入請求 </param>
        /// <returns> 登入結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AllowAnonymous]
        public async Task<BusinessLogicResponse<LoginResponse>> Login(LoginRequest loginRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<LoginResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(loginRequest.Username)) throw new BusinessException(message: "使用者名稱不能為空");
                if (string.IsNullOrEmpty(loginRequest.Password)) throw new BusinessException(message: "密碼不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                loginRequest.Username = sanitizer.Sanitize(loginRequest.Username);
                loginRequest.Username = HttpUtility.UrlDecode(loginRequest.Username);

                // ========= 業務邏輯執行 =========
                LoginResponse loginResult = await _securityRepository.AuthenticateAsync(loginRequest.Username, loginRequest.Password);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("使用者登入", "AUTH_LOGIN", "使用者：" + loginRequest.Username + " 登入 且 " + (loginResult.Success ? "成功" : "失敗"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = loginResult.Success ? "success" : "error";
                response.Message = loginResult.Success ? "登入成功" : "登入失敗";
                response.Data = loginResult;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 登出：使用者
        /// </summary>
        /// <param name="logoutRequest"> 登出請求 </param>
        /// <returns> 登出結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<BusinessLogicResponse<LogoutResponse>> Logout(LogoutRequest logoutRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<LogoutResponse> response = new();
            try
            {
                // ========= 業務邏輯執行 =========
                LogoutResponse logoutResult = await _securityRepository.LogoutAsync(logoutRequest.Token);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("使用者登出", "AUTH_LOGOUT", "使用者登出 且 " + (logoutResult.Success ? "成功" : "失敗"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = logoutResult.Success ? "success" : "error";
                response.Message = logoutResult.Success ? "登出成功" : "登出失敗";
                response.Data = logoutResult;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 刷新：JWT Token
        /// </summary>
        /// <param name="refreshRequest"> 刷新請求 </param>
        /// <returns> 新 Token </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AllowAnonymous]
        public async Task<BusinessLogicResponse<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest refreshRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<RefreshTokenResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(refreshRequest.RefreshToken)) throw new BusinessException(message: "刷新令牌不能為空");

                // ========= 業務邏輯執行 =========
                RefreshTokenResponse refreshResult = await _securityRepository.RefreshTokenAsync(refreshRequest.RefreshToken);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("Token刷新", "AUTH_REFRESH", "Token刷新 且 " + (refreshResult.Success ? "成功" : "失敗"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = refreshResult.Success ? "success" : "error";
                response.Message = refreshResult.Success ? "刷新成功" : "刷新失敗";
                response.Data = refreshResult;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        /// <summary>
        /// 驗證：JWT Token
        /// </summary>
        /// <param name="validateRequest"> 驗證請求 </param>
        /// <returns> 驗證結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AllowAnonymous]
        public async Task<BusinessLogicResponse<ValidateTokenResponse>> ValidateToken(ValidateTokenRequest validateRequest)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ValidateTokenResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(validateRequest.Token)) throw new BusinessException(message: "令牌不能為空");

                // ========= 業務邏輯執行 =========
                ValidateTokenResponse validateResult = await _securityRepository.ValidateTokenAsync(validateRequest.Token);

                // ========= 審計軌跡記錄 =========
                await CreateAuditTrail("Token驗證", "AUTH_VALIDATE", "Token驗證 且 " + (validateResult.IsValid ? "有效" : "無效"));
                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "驗證完成";
                response.Data = validateResult;
            }
            catch (BusinessException be)
            {
                response.Status = "error";
                response.Message = be.Message;
                await uow.RollbackAsync();
            }
            catch (Exception e)
            {
                _log.Error("An unexpected error occurred: {StackTrace}", e.StackTrace);
                await uow.RollbackAsync();
                throw new UserFriendlyException(e.Message);
            }

            return response;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 儲存操作紀錄至審計軌跡
        /// </summary>
        /// <param name="target"> 操作目標 </param>
        /// <param name="type"> 操作類型 </param>
        /// <param name="description"> 操作描述 </param>
        /// <returns> Task </returns>
        private async Task CreateAuditTrail(string target, string type, string description)
        {
            await _auditTrailService.CreateAsync(
                target: "SmartNameplate-系統管理-" + target,
                type: type,
                description: description
                );
        }

        #endregion Private Methods
    }
} 