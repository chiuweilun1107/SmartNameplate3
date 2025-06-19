//-----
// <copyright file="TemplateItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates
{
    /// <summary>
    /// 模板基本資料項目
    /// </summary>
    public class TemplateItem
    {
        #region Properties

        /// <summary>
        /// 模板 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

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
        /// 組織 ID
        /// </summary>
        [JsonProperty("organizationId")]
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 建立者 ID
        /// </summary>
        [JsonProperty("creatorId")]
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 建立者使用者 ID (相容性屬性)
        /// </summary>
        [JsonProperty("creatorUserId")]
        public Guid? CreatorUserId => CreatorId;

        /// <summary>
        /// 啟用狀態 (相容性屬性)
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable => true;

        /// <summary>
        /// 是否公開
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [JsonProperty("lastModificationTime")]
        public DateTime LastModificationTime { get; set; }

        #endregion Properties
    }
} 