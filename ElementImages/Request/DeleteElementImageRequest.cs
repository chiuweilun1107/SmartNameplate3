//-----
// <copyright file="DeleteElementImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request
{
    /// <summary>
    /// 刪除元素圖片請求
    /// </summary>
    public class DeleteElementImageRequest
    {
        #region Properties

        /// <summary>
        /// 元素圖片 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        #endregion Properties
    }
} 