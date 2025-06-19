//-----
// <copyright file="UserRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Request
{
    /// <summary>
    /// 使用者單筆查詢請求
    /// </summary>
    public class UserRequest
    {
        #region Properties

        /// <summary>
        /// 使用者 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        #endregion Properties
    }
} 