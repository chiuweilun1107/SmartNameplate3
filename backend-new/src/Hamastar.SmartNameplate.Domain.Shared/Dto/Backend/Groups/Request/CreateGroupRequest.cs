//-----
// <copyright file="CreateGroupRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Groups.Request
{
    /// <summary>
    /// 新增群組請求
    /// </summary>
    public class CreateGroupRequest
    {
        #region Properties

        /// <summary>
        /// 群組名稱
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "";

        /// <summary>
        /// 顏色
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; } = "";

        #endregion Properties
    }
} 