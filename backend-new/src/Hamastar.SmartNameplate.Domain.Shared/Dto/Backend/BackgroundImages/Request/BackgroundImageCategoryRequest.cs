//-----
// <copyright file="BackgroundImageCategoryRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 背景圖片分類查詢 Request
    /// </summary>
    public class BackgroundImageCategoryRequest
    {
        #region Properties

        /// <summary>
        /// 分類名稱
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        #endregion Properties
    }
} 