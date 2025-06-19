//-----
// <copyright file="RefreshTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 重新整理權杖 Response
    /// </summary>
    public class RefreshTokenResponse
    {
        #region Properties

        /// <summary>
        /// 存取權杖
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = "";

        /// <summary>
        /// 重新整理權杖
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = "";

        /// <summary>
        /// 權杖類型
        /// </summary>
        [JsonProperty("tokenType")]
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 過期時間（秒）
        /// </summary>
        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }

        #endregion Properties
    }
} 