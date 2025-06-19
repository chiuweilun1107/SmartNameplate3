//-----
// <copyright file="GroupListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups.Request
{
    /// <summary>
    /// 群組列表查詢 Request
    /// </summary>
    public class GroupListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 使用者ID篩選
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; } = "";

        #endregion Properties
    }
} 