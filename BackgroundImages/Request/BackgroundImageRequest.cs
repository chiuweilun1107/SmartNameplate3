//-----
// <copyright file="BackgroundImageRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Request
{
    /// <summary>
    /// 背景圖片請求
    /// </summary>
    public class BackgroundImageRequest
    {
        #region Properties

        /// <summary>
        /// 背景圖片 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        #endregion Properties
    }
} 