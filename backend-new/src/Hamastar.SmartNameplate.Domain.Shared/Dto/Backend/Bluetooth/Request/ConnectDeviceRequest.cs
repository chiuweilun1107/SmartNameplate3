//-----
// <copyright file="ConnectDeviceRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Request
{
    /// <summary>
    /// 連接裝置請求
    /// </summary>
    public class ConnectDeviceRequest
    {
        #region Properties

        /// <summary>
        /// 裝置名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 藍牙地址
        /// </summary>
        [JsonProperty("bluetoothAddress")]
        public string BluetoothAddress { get; set; } = "";

        /// <summary>
        /// 原始地址
        /// </summary>
        [JsonProperty("originalAddress")]
        public string OriginalAddress { get; set; } = "";

        #endregion Properties
    }
} 