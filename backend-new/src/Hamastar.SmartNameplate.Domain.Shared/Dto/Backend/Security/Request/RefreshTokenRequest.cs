//-----
// <copyright file="RefreshTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 重新整理權杖 Request
    /// </summary>
    public class RefreshTokenRequest
    {
        #region Properties

        /// <summary>
        /// 重新整理權杖
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = "";

        #endregion Properties
    }
} 