//-----
// <copyright file="CardInstanceDataResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 卡片實例資料回應
    /// </summary>
    public class CardInstanceDataResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 卡片資料
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; } = "";

        #endregion Properties
    }
} 