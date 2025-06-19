//-----
// <copyright file="DeleteBackgroundImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 刪除背景圖片 Request
    /// </summary>
    public class DeleteBackgroundImageRequest
    {
        #region Properties

        /// <summary>
        /// 背景圖片ID
        /// </summary>
        [JsonProperty("backgroundImageId")]
        public Guid BackgroundImageId { get; set; }

        #endregion Properties
    }
} 