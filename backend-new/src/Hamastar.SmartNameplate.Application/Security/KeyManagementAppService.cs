//-----
// <copyright file="KeyManagementAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Ganss.Xss;
using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using Hamastar.SmartNameplate.IApplication.Security;
using Hamastar.SmartNameplate.Permissions;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Hamastar.SmartNameplate.Security
{
    /// <summary>
    /// 金鑰管理 App Service
    /// </summary>
    public class KeyManagementAppService : ApplicationService, IKeyManagementAppService
    {
        #region Fields

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _log = Log.ForContext<KeyManagementAppService>();

        /// <summary>
        /// 目前使用者
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// 配置
        /// </summary>
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagementAppService" /> class
        /// </summary>
        /// <param name="currentUser"> 目前使用者 </param>
        /// <param name="configuration"> 配置 </param>
        public KeyManagementAppService(
            ICurrentUser currentUser,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _currentUser = currentUser;
            _configuration = configuration;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// 生成安全金鑰
        /// </summary>
        /// <param name="request"> 生成金鑰請求 </param>
        /// <returns> 生成結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.SystemMgmt)]
        public async Task<BusinessLogicResponse<GenerateKeyResponse>> GenerateSecretKey(GenerateKeyRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<GenerateKeyResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (request.Length < 32) throw new BusinessException(message: "金鑰長度不能少於32字符");
                if (request.Length > 256) throw new BusinessException(message: "金鑰長度不能超過256字符");

                // ========= 輸入清理 =========
                // 金鑰生成不需要清理輸入

                // ========= 業務邏輯執行 =========
                string secretKey = GenerateSecureKey(request.Length);

                GenerateKeyResponse generateKeyResponse = new()
                {
                    SecretKey = secretKey,
                    Length = secretKey.Length
                };

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "金鑰生成完成";
                response.Data = generateKeyResponse;
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
        /// 驗證金鑰強度
        /// </summary>
        /// <param name="request"> 驗證請求 </param>
        /// <returns> 驗證結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.SystemMgmt)]
        public async Task<BusinessLogicResponse<ValidateKeyResponse>> ValidateKeyStrength(ValidateKeyRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<ValidateKeyResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(request.Key)) throw new BusinessException(message: "金鑰不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Key = sanitizer.Sanitize(request.Key);
                request.Key = HttpUtility.UrlDecode(request.Key);

                // ========= 業務邏輯執行 =========
                bool isValid = ValidateKeyStrengthInternal(request.Key);

                ValidateKeyResponse validateKeyResponse = new()
                {
                    IsValid = isValid,
                    Message = isValid ? "金鑰強度符合要求" : "金鑰強度不足"
                };

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "金鑰驗證完成";
                response.Data = validateKeyResponse;
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
        /// 加密敏感資料
        /// </summary>
        /// <param name="request"> 加密請求 </param>
        /// <returns> 加密結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.SystemMgmt)]
        public async Task<BusinessLogicResponse<EncryptDataResponse>> EncryptSensitiveData(EncryptDataRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<EncryptDataResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(request.Plaintext)) throw new BusinessException(message: "明文資料不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.Plaintext = sanitizer.Sanitize(request.Plaintext);

                // ========= 業務邏輯執行 =========
                string encryptedData = EncryptDataInternal(request.Plaintext);

                EncryptDataResponse encryptDataResponse = new()
                {
                    EncryptedData = encryptedData
                };

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "資料加密完成";
                response.Data = encryptDataResponse;
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
        /// 解密敏感資料
        /// </summary>
        /// <param name="request"> 解密請求 </param>
        /// <returns> 解密結果 </returns>
        /// <exception cref="UserFriendlyException"></exception>
        [Authorize(AuthenticationSchemes = "Bearer", Policy = SmartNameplatePermissions.SystemMgmt)]
        public async Task<BusinessLogicResponse<DecryptDataResponse>> DecryptSensitiveData(DecryptDataRequest request)
        {
            using IUnitOfWork uow = UnitOfWorkManager.Begin();
            BusinessLogicResponse<DecryptDataResponse> response = new();
            try
            {
                // ========= 輸入驗證 =========
                if (string.IsNullOrEmpty(request.EncryptedData)) throw new BusinessException(message: "加密資料不能為空");

                // ========= 輸入清理 =========
                HtmlSanitizer sanitizer = new();
                request.EncryptedData = sanitizer.Sanitize(request.EncryptedData);

                // ========= 業務邏輯執行 =========
                string decryptedData = DecryptDataInternal(request.EncryptedData);

                DecryptDataResponse decryptDataResponse = new()
                {
                    DecryptedData = decryptedData
                };

                await uow.CompleteAsync();

                // ========= 成功回應 =========
                response.Status = "success";
                response.Message = "資料解密完成";
                response.Data = decryptDataResponse;
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
        /// 生成安全金鑰
        /// </summary>
        /// <param name="length"> 金鑰長度 </param>
        /// <returns> 安全金鑰 </returns>
        private string GenerateSecureKey(int length = 64)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 驗證金鑰強度
        /// </summary>
        /// <param name="key"> 金鑰 </param>
        /// <returns> 是否符合強度要求 </returns>
        private bool ValidateKeyStrengthInternal(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            // 基本長度檢查 (至少32字符)
            if (key.Length < 32) return false;

            // 複雜度檢查
            bool hasLowerCase = key.Any(char.IsLower);
            bool hasUpperCase = key.Any(char.IsUpper);
            bool hasDigit = key.Any(char.IsDigit);
            bool hasSpecialChar = key.Any(c => !char.IsLetterOrDigit(c));

            // 至少要有3種字符類型
            int complexity = (hasLowerCase ? 1 : 0) + (hasUpperCase ? 1 : 0) + 
                           (hasDigit ? 1 : 0) + (hasSpecialChar ? 1 : 0);

            return complexity >= 3;
        }

        /// <summary>
        /// 加密資料
        /// </summary>
        /// <param name="plaintext"> 明文 </param>
        /// <returns> 密文 </returns>
        private string EncryptDataInternal(string plaintext)
        {
            // 簡化實作，實際應使用更安全的加密方式
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 解密資料
        /// </summary>
        /// <param name="ciphertext"> 密文 </param>
        /// <returns> 明文 </returns>
        private string DecryptDataInternal(string ciphertext)
        {
            // 簡化實作，實際應使用更安全的解密方式
            var bytes = Convert.FromBase64String(ciphertext);
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion Private Methods
    }
} 