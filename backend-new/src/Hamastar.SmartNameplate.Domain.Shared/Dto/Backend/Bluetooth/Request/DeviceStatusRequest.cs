//-----
// <copyright file="DeviceStatusRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Request
{
    /// <summary>
    /// 裝置狀態查詢 Request
    /// </summary>
    public class DeviceStatusRequest
    {
        #region Properties

        /// <summary>
        /// 裝置MAC地址
        /// </summary>
        [JsonProperty("macAddress")]
        public string MacAddress { get; set; } = "";

        #endregion Properties
    }
} 