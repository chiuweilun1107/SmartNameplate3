//-----
// <copyright file="DeleteBackgroundImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response
{
    /// <summary>
    /// 刪除背景圖片 Response
    /// </summary>
    public class DeleteBackgroundImageResponse
    {
        #region Properties

        /// <summary>
        /// 操作結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 