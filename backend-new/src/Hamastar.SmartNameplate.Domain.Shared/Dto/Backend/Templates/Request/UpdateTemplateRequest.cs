//-----
// <copyright file="UpdateTemplateRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates.Request
{
    /// <summary>
    /// 更新模板請求
    /// </summary>
    public class UpdateTemplateRequest
    {
        #region Properties

        /// <summary>
        /// 模板名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        /// <summary>
        /// 縮圖 URL
        /// </summary>
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; } = "";

        /// <summary>
        /// A面縮圖
        /// </summary>
        [JsonProperty("thumbnailA")]
        public string ThumbnailA { get; set; } = "";

        /// <summary>
        /// B面縮圖
        /// </summary>
        [JsonProperty("thumbnailB")]
        public string ThumbnailB { get; set; } = "";

        /// <summary>
        /// A面佈局資料
        /// </summary>
        [JsonProperty("layoutDataA")]
        public string LayoutDataA { get; set; } = "";

        /// <summary>
        /// B面佈局資料
        /// </summary>
        [JsonProperty("layoutDataB")]
        public string LayoutDataB { get; set; } = "";

        /// <summary>
        /// 尺寸資訊
        /// </summary>
        [JsonProperty("dimensions")]
        public string Dimensions { get; set; } = "";

        /// <summary>
        /// 是否公開
        /// </summary>
        [JsonProperty("isPublic")]
        public bool? IsPublic { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("isActive")]
        public bool? IsActive { get; set; }

        #endregion Properties
    }
} 