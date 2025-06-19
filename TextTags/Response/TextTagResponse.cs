//-----
// <copyright file="TextTagResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 文字標籤回應
    /// </summary>
    public class TextTagResponse
    {
        #region Properties

        /// <summary>
        /// 文字標籤資料
        /// </summary>
        [JsonProperty("textTag")]
        public TextTagItem TextTag { get; set; } = new();

        #endregion Properties
    }
} 