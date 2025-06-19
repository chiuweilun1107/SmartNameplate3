//-----
// <copyright file="DeleteCardInstanceRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Request
{
    /// <summary>
    /// 刪除卡片實例請求
    /// </summary>
    public class DeleteCardInstanceRequest
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