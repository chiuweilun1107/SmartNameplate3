//-----
// <copyright file="EncryptDataResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 加密資料 Response
    /// </summary>
    public class EncryptDataResponse
    {
        #region Properties

        /// <summary>
        /// 加密後的資料
        /// </summary>
        [JsonProperty("encryptedData")]
        public string EncryptedData { get; set; } = "";

        /// <summary>
        /// 加密時間
        /// </summary>
        [JsonProperty("encryptedAt")]
        public DateTime EncryptedAt { get; set; } = DateTime.UtcNow;

        #endregion Properties
    }
} 