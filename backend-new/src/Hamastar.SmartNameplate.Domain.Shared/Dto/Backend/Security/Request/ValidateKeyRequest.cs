//-----
// <copyright file="ValidateKeyRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 驗證金鑰 Request
    /// </summary>
    public class ValidateKeyRequest
    {
        #region Properties

        /// <summary>
        /// 要驗證的金鑰
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        #endregion Properties
    }
} 