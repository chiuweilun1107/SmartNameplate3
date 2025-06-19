//-----
// <copyright file="DecryptDataResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 解密資料 Response
    /// </summary>
    public class DecryptDataResponse
    {
        #region Properties

        /// <summary>
        /// 解密後的資料
        /// </summary>
        [JsonProperty("decryptedData")]
        public string DecryptedData { get; set; } = "";

        /// <summary>
        /// 解密時間
        /// </summary>
        [JsonProperty("decryptedAt")]
        public DateTime DecryptedAt { get; set; } = DateTime.UtcNow;

        #endregion Properties
    }
} 