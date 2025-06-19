//-----
// <copyright file="CreateTextTagResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 新增文字標籤回應
    /// </summary>
    public class CreateTextTagResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 文字標籤 ID
        /// </summary>
        [JsonProperty("textTagId")]
        public Guid TextTagId { get; set; }

        #endregion Properties
    }
} 