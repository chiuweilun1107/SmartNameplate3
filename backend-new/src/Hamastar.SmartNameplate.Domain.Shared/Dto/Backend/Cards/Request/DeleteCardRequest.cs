//-----
// <copyright file="DeleteCardRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Cards.Request
{
    /// <summary>
    /// 刪除卡片 Request
    /// </summary>
    public class DeleteCardRequest
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