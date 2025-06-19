//-----
// <copyright file="CreateTextTagRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 新增文字標籤請求
    /// </summary>
    public class CreateTextTagRequest
    {
        #region Properties

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

        #endregion Properties
    }
} 