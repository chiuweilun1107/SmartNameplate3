//-----
// <copyright file="TemplateListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates.Request
{
    /// <summary>
    /// 模板列表查詢 Request
    /// </summary>
    public class TemplateListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 分類篩選
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        /// <summary>
        /// 頁面索引 (從0開始)
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; } = 0;

        /// <summary>
        /// 是否只顯示公開模板
        /// </summary>
        [JsonProperty("isPublicOnly")]
        public bool IsPublicOnly { get; set; } = false;

        #endregion Properties
    }
} 