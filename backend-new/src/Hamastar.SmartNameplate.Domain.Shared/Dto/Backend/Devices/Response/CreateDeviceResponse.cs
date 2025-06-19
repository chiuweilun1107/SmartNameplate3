//-----
// <copyright file="CreateDeviceResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Devices.Response
{
    /// <summary>
    /// 新增裝置回應
    /// </summary>
    public class CreateDeviceResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 裝置 ID
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        #endregion Properties
    }
} 