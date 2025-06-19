//-----
// <copyright file="DeleteGroupRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;
using System;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups.Request
{
    /// <summary>
    /// 刪除群組 Request
    /// </summary>
    public class DeleteGroupRequest
    {
        #region Properties

        /// <summary>
        /// 群組ID
        /// </summary>
        [JsonProperty("groupId")]
        public Guid GroupId { get; set; }

        #endregion Properties
    }
} 