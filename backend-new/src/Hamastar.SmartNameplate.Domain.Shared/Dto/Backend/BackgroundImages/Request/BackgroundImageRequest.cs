//-----
// <copyright file="BackgroundImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 背景圖片查詢 Request
    /// </summary>
    public class BackgroundImageRequest
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