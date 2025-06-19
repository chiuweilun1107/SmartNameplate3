//-----
// <copyright file="ElementImageListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request
{
    /// <summary>
    /// 元素圖片列表查詢 Request
    /// </summary>
    public class ElementImageListRequest : PageRequest
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

        #endregion Properties
    }
} 