//-----
// <copyright file="TextTagListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 文字標籤列表回應
    /// </summary>
    public class TextTagListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 文字標籤列表
        /// </summary>
        [JsonProperty("items")]
        public List<TextTagItem> Items { get; set; } = new();

        #endregion Properties
    }
} 