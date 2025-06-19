//-----
// <copyright file="CreateUserRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Request
{
    /// <summary>
    /// 新增使用者請求
    /// </summary>
    public class CreateUserRequest
    {
        #region Properties

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 密碼
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; } = "";

        /// <summary>
        /// 角色
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = "User";

        #endregion Properties
    }
} 