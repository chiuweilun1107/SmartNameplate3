//-----
// <copyright file="SmartNameplatePermissionDefinitionProvider.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Hamastar.SmartNameplate.Permissions;

/// <summary>
/// SmartNameplate 權限定義提供者
/// </summary>
public class SmartNameplatePermissionDefinitionProvider : PermissionDefinitionProvider
{
    #region Public Methods

    /// <summary>
    /// 定義權限
    /// </summary>
    /// <param name="context"> 權限定義上下文 </param>
    public override void Define(IPermissionDefinitionContext context)
    {
        var smartNameplateGroup = context.AddGroup(SmartNameplatePermissions.GroupName, L("Permission:SmartNameplate"));

        // ========= 系統管理 =========
        DefineSystemManagementPermissions(smartNameplateGroup);

        // ========= 使用者管理 =========
        DefineUserManagementPermissions(smartNameplateGroup);

        // ========= 裝置管理 =========
        DefineDeviceManagementPermissions(smartNameplateGroup);

        // ========= 卡片管理 =========
        DefineCardManagementPermissions(smartNameplateGroup);

        // ========= 模板管理 =========
        DefineTemplateManagementPermissions(smartNameplateGroup);

        // ========= 群組管理 =========
        DefineGroupManagementPermissions(smartNameplateGroup);

        // ========= 部署管理 =========
        DefineDeployManagementPermissions(smartNameplateGroup);

        // ========= 藍牙服務 =========
        DefineBluetoothManagementPermissions(smartNameplateGroup);

        // ========= 安全服務 =========
        DefineSecurityManagementPermissions(smartNameplateGroup);

        // ========= 審計軌跡 =========
        DefineAuditTrailManagementPermissions(smartNameplateGroup);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// 定義系統管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineSystemManagementPermissions(PermissionGroupDefinition group)
    {
        group.AddPermission(SmartNameplatePermissions.SystemMgmt, L("Permission:SystemMgmt"));
    }

    /// <summary>
    /// 定義使用者管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineUserManagementPermissions(PermissionGroupDefinition group)
    {
        var userMgmt = group.AddPermission(SmartNameplatePermissions.UserMgmt, L("Permission:UserMgmt"));
        userMgmt.AddChild(SmartNameplatePermissions.UserMgmt_View, L("Permission:UserMgmt.View"));
        userMgmt.AddChild(SmartNameplatePermissions.UserMgmt_Create, L("Permission:UserMgmt.Create"));
        userMgmt.AddChild(SmartNameplatePermissions.UserMgmt_Update, L("Permission:UserMgmt.Update"));
        userMgmt.AddChild(SmartNameplatePermissions.UserMgmt_Delete, L("Permission:UserMgmt.Delete"));
    }

    /// <summary>
    /// 定義裝置管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineDeviceManagementPermissions(PermissionGroupDefinition group)
    {
        var deviceMgmt = group.AddPermission(SmartNameplatePermissions.DeviceMgmt, L("Permission:DeviceMgmt"));
        deviceMgmt.AddChild(SmartNameplatePermissions.DeviceMgmt_View, L("Permission:DeviceMgmt.View"));
        deviceMgmt.AddChild(SmartNameplatePermissions.DeviceMgmt_Create, L("Permission:DeviceMgmt.Create"));
        deviceMgmt.AddChild(SmartNameplatePermissions.DeviceMgmt_Update, L("Permission:DeviceMgmt.Update"));
        deviceMgmt.AddChild(SmartNameplatePermissions.DeviceMgmt_Delete, L("Permission:DeviceMgmt.Delete"));
        deviceMgmt.AddChild(SmartNameplatePermissions.DeviceMgmt_Connect, L("Permission:DeviceMgmt.Connect"));
    }

    /// <summary>
    /// 定義卡片管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineCardManagementPermissions(PermissionGroupDefinition group)
    {
        var cardMgmt = group.AddPermission(SmartNameplatePermissions.CardMgmt, L("Permission:CardMgmt"));
        cardMgmt.AddChild(SmartNameplatePermissions.CardMgmt_View, L("Permission:CardMgmt.View"));
        cardMgmt.AddChild(SmartNameplatePermissions.CardMgmt_Create, L("Permission:CardMgmt.Create"));
        cardMgmt.AddChild(SmartNameplatePermissions.CardMgmt_Update, L("Permission:CardMgmt.Update"));
        cardMgmt.AddChild(SmartNameplatePermissions.CardMgmt_Delete, L("Permission:CardMgmt.Delete"));
        cardMgmt.AddChild(SmartNameplatePermissions.CardMgmt_Deploy, L("Permission:CardMgmt.Deploy"));
    }

    /// <summary>
    /// 定義模板管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineTemplateManagementPermissions(PermissionGroupDefinition group)
    {
        var templateMgmt = group.AddPermission(SmartNameplatePermissions.TemplateMgmt, L("Permission:TemplateMgmt"));
        templateMgmt.AddChild(SmartNameplatePermissions.TemplateMgmt_View, L("Permission:TemplateMgmt.View"));
        templateMgmt.AddChild(SmartNameplatePermissions.TemplateMgmt_Create, L("Permission:TemplateMgmt.Create"));
        templateMgmt.AddChild(SmartNameplatePermissions.TemplateMgmt_Update, L("Permission:TemplateMgmt.Update"));
        templateMgmt.AddChild(SmartNameplatePermissions.TemplateMgmt_Delete, L("Permission:TemplateMgmt.Delete"));
    }

    /// <summary>
    /// 定義群組管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineGroupManagementPermissions(PermissionGroupDefinition group)
    {
        var groupMgmt = group.AddPermission(SmartNameplatePermissions.GroupMgmt, L("Permission:GroupMgmt"));
        groupMgmt.AddChild(SmartNameplatePermissions.GroupMgmt_View, L("Permission:GroupMgmt.View"));
        groupMgmt.AddChild(SmartNameplatePermissions.GroupMgmt_Create, L("Permission:GroupMgmt.Create"));
        groupMgmt.AddChild(SmartNameplatePermissions.GroupMgmt_Update, L("Permission:GroupMgmt.Update"));
        groupMgmt.AddChild(SmartNameplatePermissions.GroupMgmt_Delete, L("Permission:GroupMgmt.Delete"));
    }

    /// <summary>
    /// 定義部署管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineDeployManagementPermissions(PermissionGroupDefinition group)
    {
        var deployMgmt = group.AddPermission(SmartNameplatePermissions.DeployMgmt, L("Permission:DeployMgmt"));
        deployMgmt.AddChild(SmartNameplatePermissions.DeployMgmt_View, L("Permission:DeployMgmt.View"));
        deployMgmt.AddChild(SmartNameplatePermissions.DeployMgmt_Execute, L("Permission:DeployMgmt.Execute"));
        deployMgmt.AddChild(SmartNameplatePermissions.DeployMgmt_Cancel, L("Permission:DeployMgmt.Cancel"));
        deployMgmt.AddChild(SmartNameplatePermissions.DeployMgmt_History, L("Permission:DeployMgmt.History"));
    }

    /// <summary>
    /// 定義藍牙管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineBluetoothManagementPermissions(PermissionGroupDefinition group)
    {
        var bluetoothMgmt = group.AddPermission(SmartNameplatePermissions.BluetoothMgmt, L("Permission:BluetoothMgmt"));
        bluetoothMgmt.AddChild(SmartNameplatePermissions.BluetoothMgmt_Scan, L("Permission:BluetoothMgmt.Scan"));
        bluetoothMgmt.AddChild(SmartNameplatePermissions.BluetoothMgmt_Connect, L("Permission:BluetoothMgmt.Connect"));
        bluetoothMgmt.AddChild(SmartNameplatePermissions.BluetoothMgmt_Transfer, L("Permission:BluetoothMgmt.Transfer"));
    }

    /// <summary>
    /// 定義安全管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineSecurityManagementPermissions(PermissionGroupDefinition group)
    {
        var securityMgmt = group.AddPermission(SmartNameplatePermissions.SecurityMgmt, L("Permission:SecurityMgmt"));
        securityMgmt.AddChild(SmartNameplatePermissions.SecurityMgmt_View, L("Permission:SecurityMgmt.View"));
        securityMgmt.AddChild(SmartNameplatePermissions.SecurityMgmt_Manage, L("Permission:SecurityMgmt.Manage"));
    }

    /// <summary>
    /// 定義審計軌跡管理權限
    /// </summary>
    /// <param name="group"> 權限群組 </param>
    private static void DefineAuditTrailManagementPermissions(PermissionGroupDefinition group)
    {
        var auditTrailMgmt = group.AddPermission(SmartNameplatePermissions.AuditTrailMgmt, L("Permission:AuditTrailMgmt"));
        auditTrailMgmt.AddChild(SmartNameplatePermissions.AuditTrailMgmt_View, L("Permission:AuditTrailMgmt.View"));
        auditTrailMgmt.AddChild(SmartNameplatePermissions.AuditTrailMgmt_Export, L("Permission:AuditTrailMgmt.Export"));
    }

    /// <summary>
    /// 本地化字串輔助方法
    /// </summary>
    /// <param name="name"> 本地化鍵值 </param>
    /// <returns> 本地化字串 </returns>
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SmartNameplateResource>(name);
    }

    #endregion Private Methods
} 