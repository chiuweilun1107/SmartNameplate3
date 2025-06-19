//-----
// <copyright file="GenerateKeyRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 生成金鑰 Request
    /// </summary>
    public class GenerateKeyRequest
    {
        #region Properties

        /// <summary>
        /// 金鑰長度
        /// </summary>
        [JsonProperty("length")]
        public int Length { get; set; } = 64;

        #endregion Properties
    }
} 