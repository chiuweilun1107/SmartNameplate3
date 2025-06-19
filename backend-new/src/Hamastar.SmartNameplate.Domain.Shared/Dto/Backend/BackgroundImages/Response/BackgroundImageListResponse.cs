//-----
// <copyright file="BackgroundImageListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response
{
    /// <summary>
    /// 背景圖片列表回應
    /// </summary>
    public class BackgroundImageListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 背景圖片列表
        /// </summary>
        [JsonProperty("items")]
        public List<BackgroundImageItem> Items { get; set; } = new();

        #endregion Properties
    }
} 