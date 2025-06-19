//-----
// <copyright file="ElementImageListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Response
{
    /// <summary>
    /// 元素圖片列表回應
    /// </summary>
    public class ElementImageListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 元素圖片列表
        /// </summary>
        [JsonProperty("items")]
        public List<ElementImageItem> Items { get; set; } = new();

        #endregion Properties
    }
} 