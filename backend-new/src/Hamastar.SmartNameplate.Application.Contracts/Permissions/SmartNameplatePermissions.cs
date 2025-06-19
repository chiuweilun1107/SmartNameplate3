//-----
// <copyright file="SmartNameplatePermissions.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Dto.Backend.Permission;
using System.Collections.Generic;

namespace Hamastar.SmartNameplate.Permissions;

/// <summary>
/// SmartNameplate 權限常數定義
/// </summary>
public static class SmartNameplatePermissions
{
    #region 權限 Fields

    /// <summary>
    /// 專案群組名稱
    /// </summary>
    public const string GroupName = "SmartNameplate";

    // 【後台管理系統】
    public const string BackendPlat = GroupName + ".BackendPlat";

    // ========= 系統管理 =========
    public const string SystemMgmt = GroupName + ".SystemMgmt";

    // ================== 使用者管理 ==================
    public const string UserMgmt = GroupName + ".UserMgmt";
    public const string UserMgmt_View = UserMgmt + ".View";
    public const string UserMgmt_Create = UserMgmt + ".Create";
    public const string UserMgmt_Update = UserMgmt + ".Update";
    public const string UserMgmt_Delete = UserMgmt + ".Delete";

    // ================== 裝置管理 ==================
    public const string DeviceMgmt = GroupName + ".DeviceMgmt";
    public const string DeviceMgmt_View = DeviceMgmt + ".View";
    public const string DeviceMgmt_Create = DeviceMgmt + ".Create";
    public const string DeviceMgmt_Update = DeviceMgmt + ".Update";
    public const string DeviceMgmt_Delete = DeviceMgmt + ".Delete";
    public const string DeviceMgmt_Connect = DeviceMgmt + ".Connect";

    // ================== 卡片管理 ==================
    public const string CardMgmt = GroupName + ".CardMgmt";
    public const string CardMgmt_View = CardMgmt + ".View";
    public const string CardMgmt_Create = CardMgmt + ".Create";
    public const string CardMgmt_Update = CardMgmt + ".Update";
    public const string CardMgmt_Delete = CardMgmt + ".Delete";
    public const string CardMgmt_Deploy = CardMgmt + ".Deploy";

    // ================== 模板管理 ==================
    public const string TemplateMgmt = GroupName + ".TemplateMgmt";
    public const string TemplateMgmt_View = TemplateMgmt + ".View";
    public const string TemplateMgmt_Create = TemplateMgmt + ".Create";
    public const string TemplateMgmt_Update = TemplateMgmt + ".Update";
    public const string TemplateMgmt_Delete = TemplateMgmt + ".Delete";

    // ================== 群組管理 ==================
    public const string GroupMgmt = GroupName + ".GroupMgmt";
    public const string GroupMgmt_View = GroupMgmt + ".View";
    public const string GroupMgmt_Create = GroupMgmt + ".Create";
    public const string GroupMgmt_Update = GroupMgmt + ".Update";
    public const string GroupMgmt_Delete = GroupMgmt + ".Delete";

    // ================== 部署管理 ==================
    public const string DeployMgmt = GroupName + ".DeployMgmt";
    public const string DeployMgmt_View = DeployMgmt + ".View";
    public const string DeployMgmt_Execute = DeployMgmt + ".Execute";
    public const string DeployMgmt_Cancel = DeployMgmt + ".Cancel";
    public const string DeployMgmt_History = DeployMgmt + ".History";

    // ================== 藍牙服務 ==================
    public const string BluetoothMgmt = GroupName + ".BluetoothMgmt";
    public const string BluetoothMgmt_Scan = BluetoothMgmt + ".Scan";
    public const string BluetoothMgmt_Connect = BluetoothMgmt + ".Connect";
    public const string BluetoothMgmt_Transfer = BluetoothMgmt + ".Transfer";

    // ================== 安全服務 ==================
    public const string SecurityMgmt = GroupName + ".SecurityMgmt";
    public const string SecurityMgmt_View = SecurityMgmt + ".View";
    public const string SecurityMgmt_Manage = SecurityMgmt + ".Manage";

    // ================== 審計軌跡 ==================
    public const string AuditTrailMgmt = GroupName + ".AuditTrailMgmt";
    public const string AuditTrailMgmt_View = AuditTrailMgmt + ".View";
    public const string AuditTrailMgmt_Export = AuditTrailMgmt + ".Export";

    #endregion 權限 Fields

    #region Permission Groups

    /// <summary>
    /// 所有使用者相關權限
    /// </summary>
    public static readonly string[] UserPermissions = new[]
    {
        UserMgmt_View, UserMgmt_Create, UserMgmt_Update, UserMgmt_Delete
    };

    /// <summary>
    /// 所有裝置相關權限
    /// </summary>
    public static readonly string[] DevicePermissions = new[]
    {
        DeviceMgmt_View, DeviceMgmt_Create, DeviceMgmt_Update, DeviceMgmt_Delete, DeviceMgmt_Connect
    };

    /// <summary>
    /// 所有卡片相關權限
    /// </summary>
    public static readonly string[] CardPermissions = new[]
    {
        CardMgmt_View, CardMgmt_Create, CardMgmt_Update, CardMgmt_Delete, CardMgmt_Deploy
    };

    /// <summary>
    /// 所有模板相關權限
    /// </summary>
    public static readonly string[] TemplatePermissions = new[]
    {
        TemplateMgmt_View, TemplateMgmt_Create, TemplateMgmt_Update, TemplateMgmt_Delete
    };

    /// <summary>
    /// 所有群組相關權限
    /// </summary>
    public static readonly string[] GroupPermissions = new[]
    {
        GroupMgmt_View, GroupMgmt_Create, GroupMgmt_Update, GroupMgmt_Delete
    };

    /// <summary>
    /// 所有部署相關權限
    /// </summary>
    public static readonly string[] DeployPermissions = new[]
    {
        DeployMgmt_View, DeployMgmt_Execute, DeployMgmt_Cancel, DeployMgmt_History
    };

    /// <summary>
    /// 所有藍牙相關權限
    /// </summary>
    public static readonly string[] BluetoothPermissions = new[]
    {
        BluetoothMgmt_Scan, BluetoothMgmt_Connect, BluetoothMgmt_Transfer
    };

    /// <summary>
    /// 所有安全相關權限
    /// </summary>
    public static readonly string[] SecurityPermissions = new[]
    {
        SecurityMgmt_View, SecurityMgmt_Manage
    };

    /// <summary>
    /// 所有審計軌跡相關權限
    /// </summary>
    public static readonly string[] AuditTrailPermissions = new[]
    {
        AuditTrailMgmt_View, AuditTrailMgmt_Export
    };

    #endregion Permission Groups

    #region Public Methods

    /// <summary>
    /// 查詢：權限層級 列表 (主權限 + 子權限key值)
    /// </summary>
    /// <returns> 取得結果 </returns>
    public static List<PermissionItem> GetPermissionHierarchy()
    {
        var permissionHierarchy = new List<PermissionItem>
        {
            AddPermissionItem(UserMgmt, UserPermissions),
            AddPermissionItem(DeviceMgmt, DevicePermissions),
            AddPermissionItem(CardMgmt, CardPermissions),
            AddPermissionItem(TemplateMgmt, TemplatePermissions),
            AddPermissionItem(GroupMgmt, GroupPermissions),
            AddPermissionItem(DeployMgmt, DeployPermissions),
            AddPermissionItem(BluetoothMgmt, BluetoothPermissions),
            AddPermissionItem(SecurityMgmt, SecurityPermissions),
            AddPermissionItem(AuditTrailMgmt, AuditTrailPermissions)
        };

        return permissionHierarchy;
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// 組成：一組權限物件Entity
    /// </summary>
    /// <param name="permissionKey"> 主權限 </param>
    /// <param name="childs"> 子權限key值 </param>
    /// <returns> 取得結果 </returns>
    private static PermissionItem AddPermissionItem(string permissionKey, string[] childs)
    {
        return new PermissionItem
        {
            PermissionKey = permissionKey,
            ChildPermissions = childs
        };
    }

    #endregion Private Methods
} 