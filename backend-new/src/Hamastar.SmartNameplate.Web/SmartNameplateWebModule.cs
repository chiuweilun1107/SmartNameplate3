//-----
// <copyright file="SmartNameplateWebModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Hamastar.SmartNameplate.Localization;
using Hamastar.SmartNameplate.Web.Menus;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace Hamastar.SmartNameplate.Web;

/// <summary>
/// SmartNameplate Web 模組
/// </summary>
[DependsOn(
    typeof(SmartNameplateHttpApiModule),
    typeof(SmartNameplateApplicationModule),
    typeof(SmartNameplateEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpAccountWebIdentityServerModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
)]
public class SmartNameplateWebModule : AbpModule
{
    #region Public Methods

    /// <summary>
    /// 配置服務
    /// </summary>
    /// <param name="context"> 服務配置上下文 </param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureLocalizationServices();
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// 配置 URLs
    /// </summary>
    /// <param name="configuration"> 設定 </param>
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    /// <summary>
    /// 配置套件
    /// </summary>
    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    /// <summary>
    /// 配置認證
    /// </summary>
    /// <param name="context"> 服務配置上下文 </param>
    /// <param name="configuration"> 設定 </param>
    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "SmartNameplate";
            });
    }

    /// <summary>
    /// 配置 AutoMapper
    /// </summary>
    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SmartNameplateWebModule>();
        });
    }

    /// <summary>
    /// 配置虛擬檔案系統
    /// </summary>
    /// <param name="hostingEnvironment"> 主機環境 </param>
    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<SmartNameplateDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Hamastar.SmartNameplate.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<SmartNameplateApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Hamastar.SmartNameplate.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<SmartNameplateApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Hamastar.SmartNameplate.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<SmartNameplateWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    /// <summary>
    /// 配置本地化服務
    /// </summary>
    private void ConfigureLocalizationServices()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<SmartNameplateResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource)
                );

            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
        });
    }

    /// <summary>
    /// 配置導航服務
    /// </summary>
    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new SmartNameplateMenuContributor());
        });
    }

    /// <summary>
    /// 配置自動 API 控制器
    /// </summary>
    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(SmartNameplateApplicationModule).Assembly);
        });
    }

    /// <summary>
    /// 配置 Swagger 服務
    /// </summary>
    /// <param name="services"> 服務集合 </param>
    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SmartNameplate API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    #endregion Private Methods
} 