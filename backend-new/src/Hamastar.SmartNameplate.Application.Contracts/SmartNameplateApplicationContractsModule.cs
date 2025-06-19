//-----
// <copyright file="SmartNameplateApplicationContractsModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate 應用服務合約模組
/// </summary>
[DependsOn(
    typeof(SmartNameplateDomainSharedModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpFeatureManagementApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(AbpTenantManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class SmartNameplateApplicationContractsModule : AbpModule
{
    #region Public Methods

    /// <summary>
    /// 預先配置服務
    /// </summary>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SmartNameplateDtoExtensions.Configure();
    }

    #endregion Public Methods
} 