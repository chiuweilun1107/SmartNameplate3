//-----
// <copyright file="DeployListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Deploy.Request
{
    /// <summary>
    /// 部署歷史列表查詢 Request
    /// </summary>
    public class DeployListRequest : PageRequest
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

        /// <summary>
        /// 部署狀態篩選
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        #endregion Properties
    }
} 