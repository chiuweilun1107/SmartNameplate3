//-----
// <copyright file="TextTagRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 文字標籤查詢 Request
    /// </summary>
    public class TextTagRequest
    {
        #region Properties

        /// <summary>
        /// 文字標籤ID
        /// </summary>
        [JsonProperty("textTagId")]
        public Guid TextTagId { get; set; }

        #endregion Properties
    }
} 