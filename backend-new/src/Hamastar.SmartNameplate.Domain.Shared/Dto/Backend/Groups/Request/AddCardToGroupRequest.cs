//-----
// <copyright file="AddCardToGroupRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups.Request
{
    /// <summary>
    /// 新增卡片到群組請求
    /// </summary>
    public class AddCardToGroupRequest
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