//-----
// <copyright file="RefreshTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 刷新Token回應
    /// </summary>
    public class RefreshTokenResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 新Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } = "";

        #endregion Properties
    }
} 