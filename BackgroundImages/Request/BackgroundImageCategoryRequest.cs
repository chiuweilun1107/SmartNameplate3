//-----
// <copyright file="BackgroundImageCategoryRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 背景圖片分類請求
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