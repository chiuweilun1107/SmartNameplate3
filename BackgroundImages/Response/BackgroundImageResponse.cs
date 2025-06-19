//-----
// <copyright file="BackgroundImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.BackgroundImages.Response
{
    /// <summary>
    /// 背景圖片回應
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