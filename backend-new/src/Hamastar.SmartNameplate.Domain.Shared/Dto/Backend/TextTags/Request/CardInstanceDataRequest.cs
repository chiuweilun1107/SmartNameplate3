//-----
// <copyright file="CardInstanceDataRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 卡片實例資料查詢 Request
    /// </summary>
    public class CardInstanceDataRequest
    {
        #region Properties

        /// <summary>
        /// 卡片ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        #endregion Properties
    }
} 