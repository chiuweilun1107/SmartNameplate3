//-----
// <copyright file="BackgroundImageListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 背景圖片列表請求
    /// </summary>
    public class BackgroundImageListRequest : PageRequest
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
        /// 是否只顯示公開圖片
        /// </summary>
        [JsonProperty("publicOnly")]
        public bool PublicOnly { get; set; } = false;

        #endregion Properties
    }
} 