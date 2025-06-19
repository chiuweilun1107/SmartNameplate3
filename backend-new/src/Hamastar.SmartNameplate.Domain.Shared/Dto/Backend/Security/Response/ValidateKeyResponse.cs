//-----
// <copyright file="ValidateKeyResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 驗證金鑰 Response
    /// </summary>
    public class ValidateKeyResponse
    {
        #region Properties

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 金鑰強度分數 (0-100)
        /// </summary>
        [JsonProperty("strengthScore")]
        public int StrengthScore { get; set; }

        /// <summary>
        /// 驗證訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 