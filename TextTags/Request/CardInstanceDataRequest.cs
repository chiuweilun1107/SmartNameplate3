//-----
// <copyright file="CardInstanceDataRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 卡片實例資料請求
    /// </summary>
    public class CardInstanceDataRequest
    {
        #region Properties

        /// <summary>
        /// 卡片 ID
        /// </summary>
        [JsonProperty("cardId")]
        public Guid CardId { get; set; }

        #endregion Properties
    }
} 