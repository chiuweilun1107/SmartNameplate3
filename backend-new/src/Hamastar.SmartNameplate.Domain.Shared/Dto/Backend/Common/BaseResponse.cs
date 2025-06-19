//-----
// <copyright file="BaseResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Common
{
    /// <summary>
    /// 通用回應基底類別
    /// </summary>
    public class BaseResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// 通用建立回應基底類別
    /// </summary>
    public class BaseCreateResponse : BaseResponse
    {
        #region Properties

        /// <summary>
        /// 建立的項目 ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        #endregion Properties
    }
} 