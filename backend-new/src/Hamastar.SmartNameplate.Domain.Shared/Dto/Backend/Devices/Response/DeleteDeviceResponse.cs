//-----
// <copyright file="DeleteDeviceResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Devices.Response
{
    /// <summary>
    /// 刪除裝置 Response
    /// </summary>
    public class DeleteDeviceResponse
    {
        #region Properties

        /// <summary>
        /// 操作結果
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