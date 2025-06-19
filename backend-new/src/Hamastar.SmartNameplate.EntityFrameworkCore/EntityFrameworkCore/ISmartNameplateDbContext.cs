//-----
// <copyright file="ISmartNameplateDbContext.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Hamastar.SmartNameplate.EntityFrameworkCore;

/// <summary>
/// SmartNameplate 資料庫上下文介面
/// </summary>
[ConnectionStringName("Default")]
public interface ISmartNameplateDbContext : IEfCoreDbContext
{
    #region DbSets

    /// <summary>
    /// 使用者
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// 裝置
    /// </summary>
    DbSet<Device> Devices { get; }

    /// <summary>
    /// 卡片
    /// </summary>
    DbSet<Card> Cards { get; }

    /// <summary>
    /// 模板
    /// </summary>
    DbSet<Template> Templates { get; }

    /// <summary>
    /// 群組
    /// </summary>
    DbSet<Group> Groups { get; }

    /// <summary>
    /// 群組卡片關聯
    /// </summary>
    DbSet<GroupCard> GroupCards { get; }

    /// <summary>
    /// 部署歷史
    /// </summary>
    DbSet<DeployHistory> DeployHistories { get; }

    #endregion DbSets
} 