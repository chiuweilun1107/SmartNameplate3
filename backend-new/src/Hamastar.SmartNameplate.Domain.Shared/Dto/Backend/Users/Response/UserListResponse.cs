//-----
// <copyright file="UserListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Response
{
    /// <summary>
    /// 使用者列表查詢回應
    /// </summary>
    public class UserListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 使用者列表
        /// </summary>
        [JsonProperty("items")]
        public List<UserItemForListByPage> Items { get; set; } = new();

        #endregion Properties
    }
} 