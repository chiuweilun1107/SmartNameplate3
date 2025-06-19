//-----
// <copyright file="DeleteDeviceRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Devices.Request
{
    /// <summary>
    /// 刪除裝置 Request
    /// </summary>
    public class DeleteDeviceRequest
    {
        #region Properties

        /// <summary>
        /// 裝置ID
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        #endregion Properties
    }
} 