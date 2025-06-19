//-----
// <copyright file="ValidateTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 驗證權杖 Request
    /// </summary>
    public class ValidateTokenRequest
    {
        #region Properties

        /// <summary>
        /// 存取權杖
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = "";

        #endregion Properties
    }
} 