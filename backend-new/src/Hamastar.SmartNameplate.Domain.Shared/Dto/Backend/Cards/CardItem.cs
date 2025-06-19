//-----
// <copyright file="CardItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Cards
{
    /// <summary>
    /// 卡片基本資料項目
    /// </summary>
    public class CardItem
    {
        #region Properties

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

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
        public int Status { get; set; }

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
        public bool IsSameBothSides { get; set; }

        /// <summary>
        /// 標籤資訊
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; } = "";

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 最後修改時間
        /// </summary>
        [JsonProperty("lastModificationTime")]
        public DateTime LastModificationTime { get; set; }

        #endregion Properties
    }
} 