//-----
// <copyright file="DisconnectDeviceResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response
{
    /// <summary>
    /// 中斷連接裝置 Response
    /// </summary>
    public class DisconnectDeviceResponse
    {
        #region Properties

        /// <summary>
        /// 中斷連接結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 