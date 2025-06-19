//-----
// <copyright file="SmartNameplateDomainModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Localization;
using Volo.Abp.Domain;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate 領域模組
/// </summary>
[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(SmartNameplateDomainSharedModule)
)]
public class SmartNameplateDomainModule : AbpModule
{
    #region Public Methods

    /// <summary>
    /// 配置服務
    /// </summary>
    /// <param name="context"> 服務配置上下文 </param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<SmartNameplateResource>()
                .AddBaseTypes(typeof(AbpValidationResource));
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("SmartNameplate", typeof(SmartNameplateResource));
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SmartNameplateDomainModule>();
        });
    }

    #endregion Public Methods
} 