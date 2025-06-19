//-----
// <copyright file="TextTagListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 文字標籤列表請求
    /// </summary>
    public class TextTagListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 卡片 ID 篩選
        /// </summary>
        [JsonProperty("cardId")]
        public Guid? CardId { get; set; }

        /// <summary>
        /// 標籤類型篩選
        /// </summary>
        [JsonProperty("tagType")]
        public string TagType { get; set; } = "";

        #endregion Properties
    }
} 