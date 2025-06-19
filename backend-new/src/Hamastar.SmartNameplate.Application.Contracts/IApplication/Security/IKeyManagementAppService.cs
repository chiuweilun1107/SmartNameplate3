//-----
// <copyright file="IKeyManagementAppService.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend;
using Hamastar.SmartNameplate.Dto.Backend.Security.Request;
using Hamastar.SmartNameplate.Dto.Backend.Security.Response;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Hamastar.SmartNameplate.IApplication.Security
{
    /// <summary>
    /// 金鑰管理 App Service 介面
    /// </summary>
    public interface IKeyManagementAppService : IApplicationService
    {
        #region Methods

        /// <summary>
        /// 生成安全金鑰
        /// </summary>
        /// <param name="request"> 生成金鑰請求 </param>
        /// <returns> 生成結果 </returns>
        Task<BusinessLogicResponse<GenerateKeyResponse>> GenerateSecretKey(GenerateKeyRequest request);

        /// <summary>
        /// 驗證金鑰強度
        /// </summary>
        /// <param name="request"> 驗證請求 </param>
        /// <returns> 驗證結果 </returns>
        Task<BusinessLogicResponse<ValidateKeyResponse>> ValidateKeyStrength(ValidateKeyRequest request);

        /// <summary>
        /// 加密敏感資料
        /// </summary>
        /// <param name="request"> 加密請求 </param>
        /// <returns> 加密結果 </returns>
        Task<BusinessLogicResponse<EncryptDataResponse>> EncryptSensitiveData(EncryptDataRequest request);

        /// <summary>
        /// 解密敏感資料
        /// </summary>
        /// <param name="request"> 解密請求 </param>
        /// <returns> 解密結果 </returns>
        Task<BusinessLogicResponse<DecryptDataResponse>> DecryptSensitiveData(DecryptDataRequest request);

        #endregion Methods
    }
} 