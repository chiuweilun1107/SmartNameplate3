//-----
// <copyright file="RefreshTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 刷新Token請求
    /// </summary>
    public class RefreshTokenRequest
    {
        #region Properties

        /// <summary>
        /// 刷新Token
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = "";

        #endregion Properties
    }
} 