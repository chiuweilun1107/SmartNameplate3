//-----
// <copyright file="UpdateCardRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Cards.Request
{
    /// <summary>
    /// 更新卡片請求
    /// </summary>
    public class UpdateCardRequest
    {
        #region Properties

        /// <summary>
        /// 卡片名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        /// <summary>
        /// 卡片狀態
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// A面縮圖
        /// </summary>
        [JsonProperty("thumbnailA")]
        public string ThumbnailA { get; set; } = "";

        /// <summary>
        /// B面縮圖
        /// </summary>
        [JsonProperty("thumbnailB")]
        public string ThumbnailB { get; set; } = "";

        /// <summary>
        /// A面內容
        /// </summary>
        [JsonProperty("contentA")]
        public string ContentA { get; set; } = "";

        /// <summary>
        /// B面內容
        /// </summary>
        [JsonProperty("contentB")]
        public string ContentB { get; set; } = "";

        /// <summary>
        /// 是否雙面相同
        /// </summary>
        [JsonProperty("isSameBothSides")]
        public bool? IsSameBothSides { get; set; }

        #endregion Properties
    }
} 