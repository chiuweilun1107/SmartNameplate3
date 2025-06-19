//-----
// <copyright file="ValidateTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 驗證Token請求
    /// </summary>
    public class ValidateTokenRequest
    {
        #region Properties

        /// <summary>
        /// Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } = "";

        #endregion Properties
    }
} 