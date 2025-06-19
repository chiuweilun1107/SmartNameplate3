//-----
// <copyright file="DeployCardRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Request
{
    /// <summary>
    /// 部署卡片請求
    /// </summary>
    public class DeployCardRequest
    {
        #region Properties

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        /// <summary>
        /// 投圖面 (預設投圖到 B 面)
        /// </summary>
        [JsonProperty("side")]
        public int Side { get; set; } = 2;

        #endregion Properties
    }
} 