//-----
// <copyright file="TemplateListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates.Response
{
    /// <summary>
    /// 模板列表查詢 Response
    /// </summary>
    public class TemplateListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 模板列表
        /// </summary>
        [JsonProperty("items")]
        public List<TemplateItem> Items { get; set; } = new();

        #endregion Properties
    }
} 