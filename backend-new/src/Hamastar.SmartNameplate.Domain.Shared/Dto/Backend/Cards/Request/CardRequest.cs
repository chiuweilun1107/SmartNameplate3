//-----
// <copyright file="CardRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Cards.Request
{
    /// <summary>
    /// 卡片查詢 Request
    /// </summary>
    public class CardRequest
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