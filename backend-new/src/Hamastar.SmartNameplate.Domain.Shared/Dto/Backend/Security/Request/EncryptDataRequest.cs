//-----
// <copyright file="EncryptDataRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 加密資料 Request
    /// </summary>
    public class EncryptDataRequest
    {
        #region Properties

        /// <summary>
        /// 要加密的資料
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; } = "";

        /// <summary>
        /// 加密金鑰
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        #endregion Properties
    }
} 