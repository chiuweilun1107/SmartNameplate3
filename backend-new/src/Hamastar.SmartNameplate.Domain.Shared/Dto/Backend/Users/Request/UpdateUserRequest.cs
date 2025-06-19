//-----
// <copyright file="UpdateUserRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Request
{
    /// <summary>
    /// 更新使用者請求
    /// </summary>
    public class UpdateUserRequest
    {
        #region Properties

        /// <summary>
        /// 使用者 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 密碼 (選填，如果要更新密碼)
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; } = "";

        /// <summary>
        /// 角色
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = "";

        #endregion Properties
    }
} 