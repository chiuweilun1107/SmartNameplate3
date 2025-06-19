//-----
// <copyright file="TextTagItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags
{
    /// <summary>
    /// 文字標籤項目
    /// </summary>
    public class TextTagItem
    {
        #region Properties

        /// <summary>
        /// 文字標籤 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 元素 ID
        /// </summary>
        [JsonProperty("elementId")]
        public string ElementId { get; set; } = "";

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        /// <summary>
        /// 標籤類型
        /// </summary>
        [JsonProperty("tagType")]
        public string TagType { get; set; } = "";

        /// <summary>
        /// 自訂標籤
        /// </summary>
        [JsonProperty("customLabel")]
        public string CustomLabel { get; set; } = "";

        /// <summary>
        /// 內容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = "";

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

        #endregion Properties
    }
} 