//-----
// <copyright file="DeviceListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using System;
using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Devices.Request
{
    /// <summary>
    /// 🤖 裝置列表請求 DTO
    /// 用於裝置列表查詢的請求參數
    /// </summary>
    public class DeviceListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 搜尋關鍵字
        /// </summary>
        [JsonProperty("keyword")]
        public string? Keyword { get; set; }

        /// <summary>
        /// 裝置狀態篩選
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 群組 ID 篩選
        /// </summary>
        [JsonProperty("groupId")]
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 使用者 ID 篩選
        /// </summary>
        [JsonProperty("userId")]
        public Guid? UserId { get; set; }

        /// <summary>
        /// 是否啟用篩選
        /// </summary>
        [JsonProperty("enable")]
        public bool? Enable { get; set; }

        #endregion Properties
    }
} 