//-----
// <copyright file="SmartNameplateHttpApiModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Localization.Resources.AbpUi;
using Hamastar.SmartNameplate.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate HttpApi 模組
/// </summary>
[DependsOn(
    typeof(SmartNameplateApplicationContractsModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule)
)]
public class SmartNameplateHttpApiModule : AbpModule
{
    #region Public Methods

    /// <summary>
    /// 配置服務
    /// </summary>
    /// <param name="context"> 服務配置上下文 </param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// 配置本地化
    /// </summary>
    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<SmartNameplateResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );
        });
    }

    #endregion Private Methods
} 