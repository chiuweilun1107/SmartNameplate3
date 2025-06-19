//-----
// <copyright file="SaveCardInstanceDataRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 儲存卡片實例資料 Request
    /// </summary>
    public class SaveCardInstanceDataRequest
    {
        #region Properties

        /// <summary>
        /// 卡片ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        /// <summary>
        /// 實例資料
        /// </summary>
        [JsonProperty("instanceData")]
        public List<object> InstanceData { get; set; } = new();

        #endregion Properties
    }
} 