//-----
// <copyright file="DeviceStatusResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response
{
    /// <summary>
    /// 裝置狀態查詢 Response
    /// </summary>
    public class DeviceStatusResponse
    {
        #region Properties

        /// <summary>
        /// 裝置狀態
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = "";

        /// <summary>
        /// 電池電量
        /// </summary>
        [JsonProperty("batteryLevel")]
        public int BatteryLevel { get; set; }

        /// <summary>
        /// 是否連接
        /// </summary>
        [JsonProperty("isConnected")]
        public bool IsConnected { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 