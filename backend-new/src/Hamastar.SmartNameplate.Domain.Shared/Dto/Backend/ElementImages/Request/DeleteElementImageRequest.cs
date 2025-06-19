//-----
// <copyright file="DeleteElementImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Request
{
    /// <summary>
    /// 刪除元素圖片 Request
    /// </summary>
    public class DeleteElementImageRequest
    {
        #region Properties

        /// <summary>
        /// 元素圖片ID
        /// </summary>
        [JsonProperty("elementImageId")]
        public Guid ElementImageId { get; set; }

        #endregion Properties
    }
} 