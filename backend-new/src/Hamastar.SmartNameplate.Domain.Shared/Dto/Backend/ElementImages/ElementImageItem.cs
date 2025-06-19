//-----
// <copyright file="ElementImageItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages
{
    /// <summary>
    /// 元素圖片項目
    /// </summary>
    public class ElementImageItem
    {
        #region Properties

        /// <summary>
        /// 元素圖片 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        /// <summary>
        /// 圖片 URL
        /// </summary>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; } = "";

        /// <summary>
        /// 縮圖 URL
        /// </summary>
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; } = "";

        /// <summary>
        /// 分類
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        /// <summary>
        /// 是否公開
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 建立者 ID
        /// </summary>
        [JsonProperty("createdBy")]
        public Guid? CreatedBy { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        #endregion Properties
    }
} 