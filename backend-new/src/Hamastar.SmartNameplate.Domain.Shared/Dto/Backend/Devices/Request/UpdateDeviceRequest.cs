//-----
// <copyright file="UpdateDeviceRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Devices.Request
{
    /// <summary>
    /// 更新裝置請求
    /// </summary>
    public class UpdateDeviceRequest
    {
        #region Properties

        /// <summary>
        /// 裝置名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 群組 ID
        /// </summary>
        [JsonProperty("groupId")]
        public Guid? GroupId { get; set; }

        /// <summary>
        /// 自訂索引
        /// </summary>
        [JsonProperty("customIndex")]
        public int? CustomIndex { get; set; }

        #endregion Properties
    }
} 