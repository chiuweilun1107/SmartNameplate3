//-----
// <copyright file="SmartNameplateDbContext.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using Hamastar.SmartNameplate.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Hamastar.SmartNameplate.EntityFrameworkCore;

/// <summary>
/// SmartNameplate 資料庫上下文實作
/// </summary>
[ConnectionStringName("Default")]
public class SmartNameplateDbContext : AbpDbContext<SmartNameplateDbContext>, ISmartNameplateDbContext
{
    #region DbSets

    // ========= 核心實體 =========

    /// <summary>
    /// 使用者
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 裝置
    /// </summary>
    public DbSet<Device> Devices { get; set; }

    /// <summary>
    /// 卡片
    /// </summary>
    public DbSet<Card> Cards { get; set; }

    /// <summary>
    /// 模板
    /// </summary>
    public DbSet<Template> Templates { get; set; }

    /// <summary>
    /// 群組
    /// </summary>
    public DbSet<Group> Groups { get; set; }

    // ========= 關聯實體 =========

    /// <summary>
    /// 群組卡片關聯
    /// </summary>
    public DbSet<GroupCard> GroupCards { get; set; }

    // ========= 歷史記錄 =========

    /// <summary>
    /// 部署歷史
    /// </summary>
    public DbSet<DeployHistory> DeployHistories { get; set; }

    #endregion DbSets

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartNameplateDbContext" /> class
    /// </summary>
    /// <param name="options"> 資料庫上下文選項 </param>
    public SmartNameplateDbContext(DbContextOptions<SmartNameplateDbContext> options)
        : base(options)
    {
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// 配置模型建立
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */
        builder.ConfigureSmartNameplate();
    }

    #endregion Methods
}

/// <summary>
/// SmartNameplate 實體配置擴展方法
/// </summary>
public static class SmartNameplateDbContextModelCreatingExtensions
{
    #region Configuration Methods

    /// <summary>
    /// 配置 SmartNameplate 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    public static void ConfigureSmartNameplate(this ModelBuilder builder)
    {
        // 設定資料表前綴
        const string tablePrefix = "App";

        // ========= 核心實體配置 =========
        ConfigureUser(builder, tablePrefix);
        ConfigureDevice(builder, tablePrefix);
        ConfigureCard(builder, tablePrefix);
        ConfigureTemplate(builder, tablePrefix);
        ConfigureGroup(builder, tablePrefix);

        // ========= 關聯實體配置 =========
        ConfigureGroupCard(builder, tablePrefix);

        // ========= 歷史記錄配置 =========
        ConfigureDeployHistory(builder, tablePrefix);
    }

    #endregion Configuration Methods

    #region Entity Configuration Methods

    /// <summary>
    /// 配置 User 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureUser(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<User>(b =>
        {
            b.ToTable(tablePrefix + "Users");
            b.ConfigureByConvention();

            b.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(UserConsts.MaxUserNameLength);

            b.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(UserConsts.MaxPasswordHashLength);

            b.Property(x => x.Role)
                .HasMaxLength(UserConsts.MaxRoleLength);

            b.HasIndex(x => x.UserName);
        });
    }

    /// <summary>
    /// 配置 Device 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureDevice(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<Device>(b =>
        {
            b.ToTable(tablePrefix + "Devices");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(DeviceConsts.MaxNameLength);

            b.Property(x => x.BluetoothAddress)
                .IsRequired()
                .HasMaxLength(DeviceConsts.MaxBluetoothAddressLength);

            b.Property(x => x.OriginalAddress)
                .HasMaxLength(DeviceConsts.MaxOriginalAddressLength);

            // 外鍵關聯
            b.HasOne(x => x.CurrentCard)
                .WithMany()
                .HasForeignKey(x => x.CurrentCardId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(x => x.Group)
                .WithMany(x => x.Devices)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.BluetoothAddress);
            b.HasIndex(x => x.CurrentCardId);
            b.HasIndex(x => x.GroupId);
            b.HasIndex(x => x.CreatorId);
        });
    }

    /// <summary>
    /// 配置 Card 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureCard(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<Card>(b =>
        {
            b.ToTable(tablePrefix + "Cards");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(CardConsts.MaxNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(CardConsts.MaxDescriptionLength);

            b.Property(x => x.Tags)
                .HasMaxLength(CardConsts.MaxTagsLength);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreatorId);
        });
    }

    /// <summary>
    /// 配置 Template 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureTemplate(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<Template>(b =>
        {
            b.ToTable(tablePrefix + "Templates");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(TemplateConsts.MaxNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(TemplateConsts.MaxDescriptionLength);

            b.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(TemplateConsts.MaxCategoryLength);

            b.Property(x => x.Tags)
                .HasMaxLength(TemplateConsts.MaxTagsLength);

            // 外鍵關聯
            b.HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.Category);
            b.HasIndex(x => x.IsPublic);
            b.HasIndex(x => x.CreatorId);
        });
    }

    /// <summary>
    /// 配置 Group 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureGroup(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<Group>(b =>
        {
            b.ToTable(tablePrefix + "Groups");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(GroupConsts.MaxNameLength);

            b.Property(x => x.Description)
                .HasMaxLength(GroupConsts.MaxDescriptionLength);

            b.Property(x => x.Color)
                .HasMaxLength(GroupConsts.MaxColorLength);

            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.CreatorId);
        });
    }

    /// <summary>
    /// 配置 GroupCard 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureGroupCard(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<GroupCard>(b =>
        {
            b.ToTable(tablePrefix + "GroupCards");
            b.ConfigureByConvention();

            // 外鍵關聯
            b.HasOne(x => x.Group)
                .WithMany(x => x.GroupCards)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Card)
                .WithMany(x => x.GroupCards)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // 複合索引
            b.HasIndex(x => new { x.GroupId, x.CardId }).IsUnique();
            b.HasIndex(x => x.CreatorId);
        });
    }

    /// <summary>
    /// 配置 DeployHistory 實體
    /// </summary>
    /// <param name="builder"> 模型建構器 </param>
    /// <param name="tablePrefix"> 資料表前綴 </param>
    private static void ConfigureDeployHistory(ModelBuilder builder, string tablePrefix)
    {
        builder.Entity<DeployHistory>(b =>
        {
            b.ToTable(tablePrefix + "DeployHistories");
            b.ConfigureByConvention();

            b.Property(x => x.ErrorMessage)
                .HasMaxLength(DeployHistoryConsts.MaxErrorMessageLength);

            // 外鍵關聯
            b.HasOne(x => x.Device)
                .WithMany(x => x.DeployHistories)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Card)
                .WithMany(x => x.DeployHistories)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.DeviceId);
            b.HasIndex(x => x.CardId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreationTime);
            b.HasIndex(x => x.CreatorId);
        });
    }

    #endregion Entity Configuration Methods
}

#region Constants

/// <summary>
/// User 實體常數定義
/// </summary>
public static class UserConsts
{
    public const int MaxUserNameLength = 100;
    public const int MaxPasswordHashLength = 500;
    public const int MaxRoleLength = 50;
}

/// <summary>
/// Device 實體常數定義
/// </summary>
public static class DeviceConsts
{
    public const int MaxNameLength = 100;
    public const int MaxBluetoothAddressLength = 200;
    public const int MaxOriginalAddressLength = 200;
}

/// <summary>
/// Card 實體常數定義
/// </summary>
public static class CardConsts
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxTagsLength = 500;
}

/// <summary>
/// Template 實體常數定義
/// </summary>
public static class TemplateConsts
{
    public const int MaxNameLength = 255;
    public const int MaxDescriptionLength = 500;
    public const int MaxCategoryLength = 100;
    public const int MaxTagsLength = 500;
}

/// <summary>
/// Group 實體常數定義
/// </summary>
public static class GroupConsts
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxColorLength = 7;
}

/// <summary>
/// DeployHistory 實體常數定義
/// </summary>
public static class DeployHistoryConsts
{
    public const int MaxErrorMessageLength = 1000;
}

#endregion Constants 