//-----
// <copyright file="ElementImageResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.ElementImages.Response
{
    /// <summary>
    /// 元素圖片查詢 Response
    /// </summary>
    public class ElementImageResponse
    {
        #region Properties

        /// <summary>
        /// 元素圖片資料
        /// </summary>
        [JsonProperty("elementImage")]
        public ElementImageItem ElementImage { get; set; } = new();

        #endregion Properties
    }
}