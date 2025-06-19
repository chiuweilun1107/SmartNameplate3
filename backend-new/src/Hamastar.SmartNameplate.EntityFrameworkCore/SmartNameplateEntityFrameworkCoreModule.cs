//-----
// <copyright file="SmartNameplateEntityFrameworkCoreModule.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Hamastar.SmartNameplate.Domain;
using Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Bluetooth;
using Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Deploy;
using Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Devices;
using Hamastar.SmartNameplate.EntityFrameworkCore.Repositories.Users;

namespace Hamastar.SmartNameplate.EntityFrameworkCore
{
    /// <summary>
    /// 🤖 SmartNameplate EntityFrameworkCore 模組
    /// </summary>
    [DependsOn(
        typeof(SmartNameplateDomainModule),
        typeof(AbpEntityFrameworkCoreModule)
    )]
    public class SmartNameplateEntityFrameworkCoreModule : AbpModule
    {
        /// <summary>
        /// 配置服務
        /// </summary>
        /// <param name="context"> 服務配置上下文 </param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<SmartNameplateDbContext>(options =>
            {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            Configure<AbpDbContextOptions>(options =>
            {
                /* The main point to change your DBMS.
                 * See also SmartNameplateMigrationsDbContextFactory for EF Core tooling. */
                options.UseSqlServer();
            });

            // 註冊自定義 Repository
            context.Services.AddTransient<IBluetoothRepository, BluetoothRepository>();
            context.Services.AddTransient<IDeployRepository, DeployRepository>();
            context.Services.AddTransient<IDeviceRepository, DeviceRepository>();
            context.Services.AddTransient<IUserRepository, UserRepository>();
        }
    }
} 