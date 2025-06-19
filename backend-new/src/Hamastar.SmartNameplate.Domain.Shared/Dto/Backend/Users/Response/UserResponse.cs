//-----
// <copyright file="UserResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Response
{
    /// <summary>
    /// 使用者單筆查詢回應
    /// </summary>
    public class UserResponse
    {
        #region Properties

        /// <summary>
        /// 使用者資料
        /// </summary>
        [JsonProperty("user")]
        public UserItem User { get; set; } = new();

        #endregion Properties
    }
} 