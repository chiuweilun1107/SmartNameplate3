//-----
// <copyright file="PermissionItem.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Permission;

/// <summary>
/// 權限項目 DTO
/// </summary>
public class PermissionItem
{
    #region Properties

    /// <summary>
    /// 權限金鑰
    /// </summary>
    [JsonProperty("permissionKey")]
    public string PermissionKey { get; set; } = "";

    /// <summary>
    /// 子權限列表
    /// </summary>
    [JsonProperty("childPermissions")]
    public string[] ChildPermissions { get; set; } = new string[0];

    #endregion Properties
} 