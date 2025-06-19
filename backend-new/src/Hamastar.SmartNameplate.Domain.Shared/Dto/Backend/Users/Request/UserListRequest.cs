//-----
// <copyright file="UserListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Request
{
    /// <summary>
    /// 使用者列表查詢請求
    /// </summary>
    public class UserListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 角色篩選
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; } = "";

        #endregion Properties
    }
} 