//-----
// <copyright file="CreateUserResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Users.Response
{
    /// <summary>
    /// 新增使用者回應
    /// </summary>
    public class CreateUserResponse
    {
        #region Properties

        /// <summary>
        /// 操作結果
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 新建立的使用者 ID
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        #endregion Properties
    }
} 