//-----
// <copyright file="ElementImageCategoryRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request
{
    /// <summary>
    /// 元素圖片分類請求
    /// </summary>
    public class ElementImageCategoryRequest
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