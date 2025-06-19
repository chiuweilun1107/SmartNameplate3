//-----
// <copyright file="ValidateTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 驗證Token回應
    /// </summary>
    public class ValidateTokenResponse
    {
        #region Properties

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        #endregion Properties
    }
} 