//-----
// <copyright file="UpdateTextTagRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 更新文字標籤請求
    /// </summary>
    public class UpdateTextTagRequest
    {
        #region Properties

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