//-----
// <copyright file="SmartNameplateDomainSharedModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate 共享領域模組
/// </summary>
[DependsOn(
    typeof(AbpValidationModule)
)]
public class SmartNameplateDomainSharedModule : AbpModule
{
    #region Public Methods

    /// <summary>
    /// 配置服務
    /// </summary>
    /// <param name="context"> 服務配置上下文 </param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SmartNameplateDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<SmartNameplateResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/SmartNameplate");

            options.DefaultResourceType = typeof(SmartNameplateResource);
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("SmartNameplate", typeof(SmartNameplateResource));
        });
    }

    #endregion Public Methods
} 