//-----
// <copyright file="UserItemForList.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Users
{
    /// <summary>
    /// 使用者列表顯示項目
    /// </summary>
    public class UserItemForList
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
        /// 角色
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = "";

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        #endregion Properties
    }
} 