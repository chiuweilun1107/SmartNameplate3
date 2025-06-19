//-----
// <copyright file="CardResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Cards.Response
{
    /// <summary>
    /// 卡片查詢 Response
    /// </summary>
    public class CardResponse
    {
        #region Properties

        /// <summary>
        /// 卡片資料
        /// </summary>
        [JsonProperty("card")]
        public CardItem Card { get; set; } = new();

        #endregion Properties
    }
} 