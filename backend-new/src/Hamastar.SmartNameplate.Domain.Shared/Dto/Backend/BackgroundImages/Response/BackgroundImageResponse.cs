//-----
// <copyright file="BackgroundImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response
{
    /// <summary>
    /// 背景圖片查詢 Response
    /// </summary>
    public class BackgroundImageResponse
    {
        #region Properties

        /// <summary>
        /// 背景圖片資料
        /// </summary>
        [JsonProperty("backgroundImage")]
        public BackgroundImageItem BackgroundImage { get; set; } = new();

        #endregion Properties
    }
} 