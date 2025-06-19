//-----
// <copyright file="DeleteTextTagRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 刪除文字標籤請求
    /// </summary>
    public class DeleteTextTagRequest
    {
        #region Properties

        /// <summary>
        /// 文字標籤 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        #endregion Properties
    }
} 