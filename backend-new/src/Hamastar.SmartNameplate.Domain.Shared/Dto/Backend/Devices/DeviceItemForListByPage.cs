//-----
// <copyright file="DeviceItemForListByPage.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices
{
    /// <summary>
    /// 🤖 分頁裝置列表項目 DTO
    /// 用於分頁裝置列表的顯示
    /// </summary>
    public class DeviceItemForListByPage
    {
        #region Properties

        /// <summary>
        /// 裝置 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 裝置名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 藍牙地址
        /// </summary>
        [JsonProperty("bluetoothAddress")]
        public string BluetoothAddress { get; set; } = string.Empty;

        /// <summary>
        /// 原始地址
        /// </summary>
        [JsonProperty("originalAddress")]
        public string? OriginalAddress { get; set; }

        /// <summary>
        /// 裝置描述
        /// </summary>
        [JsonProperty("description")]
        public string? Description { get; set; }

        /// <summary>
        /// 裝置狀態
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        /// 電池電量
        /// </summary>
        [JsonProperty("batteryLevel")]
        public int? BatteryLevel { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [JsonProperty("enable")]
        public bool Enable { get; set; }

        /// <summary>
        /// 當前卡片 ID
        /// </summary>
        [JsonProperty("currentCardId")]
        public Guid? CurrentCardId { get; set; }

        /// <summary>
        /// 群組 ID
        /// </summary>
        [JsonProperty("groupId")]
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 最後連線時間
        /// </summary>
        [JsonProperty("lastConnected")]
        public DateTime LastConnected { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        [JsonProperty("lastModificationTime")]
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 建立者 ID
        /// </summary>
        [JsonProperty("creatorUserId")]
        public Guid? CreatorUserId { get; set; }

        /// <summary>
        /// 自訂排序編號
        /// </summary>
        [JsonProperty("customIndex")]
        public int? CustomIndex { get; set; }

        #endregion Properties
    }
} 