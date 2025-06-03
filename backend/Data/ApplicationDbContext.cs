using Microsoft.EntityFrameworkCore;
using SmartNameplate.Api.Entities;

namespace SmartNameplate.Api.Data;

public class ApplicationDbContext : DbContext
{
    private readonly DatabaseConfiguration _databaseConfig;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, DatabaseConfiguration databaseConfig)
        : base(options)
    {
        _databaseConfig = databaseConfig;
    }

    public DbSet<Card> Cards { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupCard> GroupCards { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<DeployHistory> DeployHistories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<BackgroundImage> BackgroundImages { get; set; }
    public DbSet<ElementImage> ElementImages { get; set; }
    public DbSet<CustomColor> CustomColors { get; set; }
    public DbSet<TextTag> TextTags { get; set; }
    public DbSet<CardTextElement> CardTextElements { get; set; }
    public DbSet<CardInstanceData> CardInstanceDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Card entity configuration
        modelBuilder.Entity<Card>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.ContentA)
                .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());
            entity.Property(e => e.ContentB)
                .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Group entity configuration
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.Color)
                .HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CreatedAt);
        });

        // GroupCard entity configuration
        modelBuilder.Entity<GroupCard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Foreign keys
            entity.HasOne(e => e.Group)
                .WithMany(g => g.GroupCards)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint
            entity.HasIndex(e => new { e.GroupId, e.CardId }).IsUnique();
        });

        // Device entity configuration
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.BluetoothAddress)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            // Foreign keys
            entity.HasOne(e => e.CurrentCard)
                .WithMany()
                .HasForeignKey(e => e.CurrentCardId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Group)
                .WithMany(g => g.Devices)
                .HasForeignKey(e => e.GroupId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => e.BluetoothAddress).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.LastConnected);
        });

        // DeployHistory entity configuration
        modelBuilder.Entity<DeployHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(500);
            entity.Property(e => e.DeployedBy)
                .HasMaxLength(100);

            // Foreign keys
            entity.HasOne(e => e.Device)
                .WithMany(d => d.DeployHistories)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.CardId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Template entity configuration
        modelBuilder.Entity<Template>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasDefaultValue("general");
            entity.Property(e => e.LayoutDataA)
                .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());
            entity.Property(e => e.LayoutDataB)
                .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());
            entity.Property(e => e.Dimensions)
                .HasColumnType(_databaseConfig.Provider.GetJsonColumnType());
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Foreign keys
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
        });

        // BackgroundImage entity configuration
        modelBuilder.Entity<BackgroundImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.ImageUrl)
                .IsRequired();
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasDefaultValue("general");
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);

            // Foreign keys
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ElementImage entity configuration
        modelBuilder.Entity<ElementImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
            entity.Property(e => e.ImageUrl)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.ThumbnailUrl)
                .HasColumnType("text");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasDefaultValue("general");
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);

            // Foreign keys
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CustomColor entity configuration
        modelBuilder.Entity<CustomColor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.ColorValue)
                .IsRequired()
                .HasMaxLength(7);
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Indexes
            entity.HasIndex(e => e.ColorValue);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.CreatedAt);
        });

        // TextTag entity configuration
        modelBuilder.Entity<TextTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ElementId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.TagType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CustomLabel)
                .HasMaxLength(200);
            entity.Property(e => e.Content)
                .HasMaxLength(1000);
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .IsRequired();

            // Foreign keys
            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.ElementId);
            entity.HasIndex(e => e.CardId);
            entity.HasIndex(e => e.TagType);
            entity.HasIndex(e => e.CreatedAt);
        });

        // CardTextElement entity configuration
        modelBuilder.Entity<CardTextElement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Side)
                .IsRequired()
                .HasMaxLength(1);
            entity.Property(e => e.ElementId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.TagType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TagLabel)
                .HasMaxLength(100);
            entity.Property(e => e.DefaultContent)
                .HasMaxLength(500);

            // Foreign keys
            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.CardId, e.Side, e.ElementId })
                .IsUnique();
            entity.HasIndex(e => e.TagType);
        });

        // CardInstanceData entity configuration
        modelBuilder.Entity<CardInstanceData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InstanceName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Side)
                .IsRequired()
                .HasMaxLength(1);
            entity.Property(e => e.TagType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.ContentValue)
                .IsRequired()
                .HasMaxLength(1000);

            // Foreign keys
            entity.HasOne(e => e.Card)
                .WithMany()
                .HasForeignKey(e => e.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.CardId, e.InstanceName, e.Side, e.TagType });
            entity.HasIndex(e => e.TagType);
        });

        // 移除種子數據 - 使用真實資料
    }
}