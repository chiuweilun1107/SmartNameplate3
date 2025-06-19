//-----
// <copyright file="ValidateTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 驗證權杖 Response
    /// </summary>
    public class ValidateTokenResponse
    {
        #region Properties

        /// <summary>
        /// 驗證結果
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// 使用者ID
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; } = "";

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; } = "";

        #endregion Properties
    }
} 