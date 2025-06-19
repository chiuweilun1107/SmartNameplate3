//-----
// <copyright file="CardListRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Newtonsoft.Json;
using Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Cards.Request
{
    /// <summary>
    /// 🤖 卡片列表查詢 Request (對應原始 CardListRequest)
    /// </summary>
    public class CardListRequest : PageRequest
    {
        #region Properties

        /// <summary>
        /// 關鍵字搜索
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; } = "";

        /// <summary>
        /// 卡片狀態篩選
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 使用者ID篩選
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; } = "";

        #endregion Properties
    }
} 