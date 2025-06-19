//-----
// <copyright file="GroupListResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups.Response
{
    /// <summary>
    /// 群組列表回應
    /// </summary>
    public class GroupListResponse : PageResponse
    {
        #region Properties

        /// <summary>
        /// 群組列表
        /// </summary>
        [JsonProperty("items")]
        public List<GroupItemForListByPage> Items { get; set; } = new();

        #endregion Properties
    }
} 