//-----
// <copyright file="DeleteTemplateRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Templates.Request
{
    /// <summary>
    /// 刪除模板 Request
    /// </summary>
    public class DeleteTemplateRequest
    {
        #region Properties

        /// <summary>
        /// 模板ID
        /// </summary>
        [JsonProperty("templateId")]
        public Guid TemplateId { get; set; }

        #endregion Properties
    }
} 