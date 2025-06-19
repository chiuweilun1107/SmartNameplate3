//-----
// <copyright file="DeployHistoryListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Newtonsoft.Json;
using System;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Deploy.Request
{
    /// <summary>
    /// 🤖 部署歷史列表請求
    /// </summary>
    public class DeployHistoryListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 裝置 ID
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid? DeviceId { get; set; }

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid? CardId { get; set; }

        /// <summary>
        /// 部署狀態
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        [JsonProperty("startTime")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        [JsonProperty("endTime")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 部署者
        /// </summary>
        [JsonProperty("deployedBy")]
        public string? DeployedBy { get; set; }

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 頁面大小
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 頁面索引 (從0開始)
        /// </summary>
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; } = 0;

        #endregion Properties
    }
} 