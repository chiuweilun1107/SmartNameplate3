//-----
// <copyright file="SmartNameplateDtoExtensions.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate DTO 擴展方法
/// </summary>
public static class SmartNameplateDtoExtensions
{
    #region Fields

    /// <summary>
    /// 一次性配置執行器
    /// </summary>
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    #endregion Fields

    #region Public Methods

    /// <summary>
    /// 配置 DTO 擴展
    /// </summary>
    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can add extension properties to DTOs
             * defined in the depended modules.
             * 
             * Example:
             * 
             * ObjectExtensionManager.Instance
             *   .AddOrUpdateProperty<IdentityUserDto, string>("Title");
             * 
             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Object-Extensions
             */
        });
    }

    #endregion Public Methods
} 