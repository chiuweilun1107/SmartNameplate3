//-----
// <copyright file="DeployItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy
{
    /// <summary>
    /// 🤖 部署項目 DTO
    /// 對應原始 DeployHistoryDto
    /// </summary>
    public class DeployItem
    {
        #region Properties

        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 裝置 ID
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 建立時間
        /// </summary>
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 部署時間
        /// </summary>
        [JsonProperty("deployedAt")]
        public DateTime? DeployedAt { get; set; }

        /// <summary>
        /// 預定時間
        /// </summary>
        [JsonProperty("scheduledAt")]
        public DateTime? ScheduledAt { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 部署者
        /// </summary>
        [JsonProperty("deployedBy")]
        public string? DeployedBy { get; set; }

        /// <summary>
        /// 是否預定
        /// </summary>
        [JsonProperty("isScheduled")]
        public bool IsScheduled { get; set; }

        /// <summary>
        /// 重試次數
        /// </summary>
        [JsonProperty("retryCount")]
        public int RetryCount { get; set; }

        #endregion Properties
    }
} 