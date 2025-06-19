//-----
// <copyright file="GenerateKeyResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> System </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 生成金鑰 Response
    /// </summary>
    public class GenerateKeyResponse
    {
        #region Properties

        /// <summary>
        /// 生成的金鑰
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; } = "";

        /// <summary>
        /// 金鑰長度
        /// </summary>
        [JsonProperty("length")]
        public int Length { get; set; }

        /// <summary>
        /// 生成時間
        /// </summary>
        [JsonProperty("generatedAt")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        #endregion Properties
    }
} 