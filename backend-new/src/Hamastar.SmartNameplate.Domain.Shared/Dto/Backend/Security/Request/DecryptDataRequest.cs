//-----
// <copyright file="DecryptDataRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 解密資料 Request
    /// </summary>
    public class DecryptDataRequest
    {
        #region Properties

        /// <summary>
        /// 要解密的加密資料
        /// </summary>
        [JsonProperty("encryptedData")]
        public string EncryptedData { get; set; } = "";

        /// <summary>
        /// 解密金鑰
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        #endregion Properties
    }
} 