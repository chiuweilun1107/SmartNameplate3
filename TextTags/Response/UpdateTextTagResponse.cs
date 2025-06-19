//-----
// <copyright file="UpdateTextTagResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.TextTags.Response
{
    /// <summary>
    /// 更新文字標籤回應
    /// </summary>
    public class UpdateTextTagResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        #endregion Properties
    }
} 