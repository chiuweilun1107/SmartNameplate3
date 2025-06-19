//-----
// <copyright file="SmartNameplateApplicationModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Hamastar.SmartNameplate.Application.Contracts;
using Hamastar.SmartNameplate.Application.BackgroundImages;
using Hamastar.SmartNameplate.Application.Bluetooth;
using Hamastar.SmartNameplate.Application.Cards;
using Hamastar.SmartNameplate.Application.Deploy;
using Hamastar.SmartNameplate.Application.Devices;
using Hamastar.SmartNameplate.Application.ElementImages;
using Hamastar.SmartNameplate.Application.Groups;
using Hamastar.SmartNameplate.Application.Security;
using Hamastar.SmartNameplate.Application.Templates;
using Hamastar.SmartNameplate.Application.TextTags;
using Hamastar.SmartNameplate.Application.Users;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.BackgroundImages;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Bluetooth;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Cards;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Deploy;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Devices;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.ElementImages;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Groups;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Security;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Templates;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.TextTags;
using Hamastar.SmartNameplate.Application.Contracts.IApplication.Users;

namespace Hamastar.SmartNameplate.Application
{
    /// <summary>
    /// 🤖 SmartNameplate Application 模組
    /// </summary>
    [DependsOn(
        typeof(SmartNameplateApplicationContractsModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule),
        typeof(AbpSettingManagementApplicationModule)
    )]
    public class SmartNameplateApplicationModule : AbpModule
    {
        /// <summary>
        /// 配置服務
        /// </summary>
        /// <param name="context"> 服務配置上下文 </param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<SmartNameplateApplicationModule>();
            });

            // 註冊 Application Services
            context.Services.AddTransient<IBackgroundImagesAppService, BackgroundImagesAppService>();
            context.Services.AddTransient<IBluetoothAppService, BluetoothAppService>();
            context.Services.AddTransient<ICardAppService, CardAppService>();
            context.Services.AddTransient<IDeployAppService, DeployAppService>();
            context.Services.AddTransient<IDeviceAppService, DeviceAppService>();
            context.Services.AddTransient<IElementImagesAppService, ElementImagesAppService>();
            context.Services.AddTransient<IGroupAppService, GroupAppService>();
            context.Services.AddTransient<IKeyManagementAppService, KeyManagementAppService>();
            context.Services.AddTransient<ISecurityAppService, SecurityAppService>();
            context.Services.AddTransient<ITemplateAppService, TemplateAppService>();
            context.Services.AddTransient<ITextTagsAppService, TextTagsAppService>();
            context.Services.AddTransient<IUserAppService, UserAppService>();
        }
    }
} 