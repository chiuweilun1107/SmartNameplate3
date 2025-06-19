//-----
// <copyright file="CreateBackgroundImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response
{
    /// <summary>
    /// 建立背景圖片 Response
    /// </summary>
    public class CreateBackgroundImageResponse
    {
        #region Properties

        /// <summary>
        /// 操作結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 背景圖片ID
        /// </summary>
        [JsonProperty("backgroundImageId")]
        public Guid BackgroundImageId { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 