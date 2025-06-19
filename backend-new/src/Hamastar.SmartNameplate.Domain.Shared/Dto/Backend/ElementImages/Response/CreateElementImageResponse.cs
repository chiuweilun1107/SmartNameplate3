//-----
// <copyright file="CreateElementImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Response
{
    /// <summary>
    /// 建立元素圖片 Response
    /// </summary>
    public class CreateElementImageResponse
    {
        #region Properties

        /// <summary>
        /// 操作結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 元素圖片ID
        /// </summary>
        [JsonProperty("elementImageId")]
        public Guid ElementImageId { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 