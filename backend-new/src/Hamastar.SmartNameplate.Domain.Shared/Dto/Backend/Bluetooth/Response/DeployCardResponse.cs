//-----
// <copyright file="DeployCardResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Bluetooth.Response
{
    /// <summary>
    /// 部署卡片 Response
    /// </summary>
    public class DeployCardResponse
    {
        #region Properties

        /// <summary>
        /// 部署結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 部署進度
        /// </summary>
        [JsonProperty("progress")]
        public int Progress { get; set; }

        /// <summary>
        /// 訊息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = "";

        #endregion Properties
    }
} 