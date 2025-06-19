//-----
// <copyright file="LoginRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 登入 Request
    /// </summary>
    public class LoginRequest
    {
        #region Properties

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [JsonProperty("username")]
        [Required(ErrorMessage = "使用者名稱為必填")]
        public string Username { get; set; } = "";

        /// <summary>
        /// 密碼
        /// </summary>
        [JsonProperty("password")]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; } = "";

        /// <summary>
        /// 記住我
        /// </summary>
        [JsonProperty("rememberMe")]
        public bool RememberMe { get; set; } = false;

        #endregion Properties
    }
} 