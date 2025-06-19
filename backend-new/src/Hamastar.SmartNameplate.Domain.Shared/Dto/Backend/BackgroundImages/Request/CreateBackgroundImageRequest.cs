//-----
// <copyright file="CreateBackgroundImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 新增背景圖片請求
    /// </summary>
    public class CreateBackgroundImageRequest
    {
        #region Properties

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
        public string Category { get; set; } = "general";

        /// <summary>
        /// 是否公開
        /// </summary>
        [JsonProperty("isPublic")]
        public bool IsPublic { get; set; } = true;

        #endregion Properties
    }
} 