//-----
// <copyright file="DeleteCardInstanceRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 刪除卡片實例 Request
    /// </summary>
    public class DeleteCardInstanceRequest
    {
        #region Properties

        /// <summary>
        /// 卡片ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        /// <summary>
        /// 實例ID
        /// </summary>
        [JsonProperty("instanceId")]
        public Guid InstanceId { get; set; }

        #endregion Properties
    }
} 