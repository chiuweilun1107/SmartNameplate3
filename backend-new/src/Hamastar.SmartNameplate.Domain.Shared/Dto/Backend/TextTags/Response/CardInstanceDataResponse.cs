//-----
// <copyright file="CardInstanceDataResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 卡片實例資料查詢 Response
    /// </summary>
    public class CardInstanceDataResponse
    {
        #region Properties

        /// <summary>
        /// 卡片實例資料列表
        /// </summary>
        [JsonProperty("instanceData")]
        public List<object> InstanceData { get; set; } = new();

        #endregion Properties
    }
} 