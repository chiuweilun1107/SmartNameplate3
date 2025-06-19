//-----
// <copyright file="TemplateCategoryRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates.Request
{
    /// <summary>
    /// 模板分類查詢 Request
    /// </summary>
    public class TemplateCategoryRequest
    {
        #region Properties

        /// <summary>
        /// 分類名稱
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; } = "";

        #endregion Properties
    }
} 