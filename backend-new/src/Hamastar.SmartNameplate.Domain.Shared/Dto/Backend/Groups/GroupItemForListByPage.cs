//-----
// <copyright file="GroupItemForListByPage.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups
{
    /// <summary>
    /// 群組分頁列表項目
    /// </summary>
    public class GroupItemForListByPage
    {
        #region Properties

        /// <summary>
        /// 群組 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        /// <summary>
        /// 顏色
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; } = "";

        /// <summary>
        /// 卡片數量
        /// </summary>
        [JsonProperty("cardCount")]
        public int CardCount { get; set; }

        /// <summary>
        /// 裝置數量
        /// </summary>
        [JsonProperty("deviceCount")]
        public int DeviceCount { get; set; }

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

        /// <summary>
        /// 建立者 ID
        /// </summary>
        [JsonProperty("creatorId")]
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 建立者使用者 ID (相容性屬性)
        /// </summary>
        [JsonProperty("creatorUserId")]
        public Guid? CreatorUserId => CreatorId;

        /// <summary>
        /// 啟用狀態 (相容性屬性)
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable => true;

        #endregion Properties
    }
} 