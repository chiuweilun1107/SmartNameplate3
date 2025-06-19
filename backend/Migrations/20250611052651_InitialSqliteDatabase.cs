using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqliteDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ThumbnailA = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailB = table.Column<string>(type: "TEXT", nullable: true),
                    ContentA = table.Column<string>(type: "TEXT", nullable: true),
                    ContentB = table.Column<string>(type: "TEXT", nullable: true),
                    IsSameBothSides = table.Column<bool>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ColorValue = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardInstanceDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    InstanceName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Side = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    TagType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContentValue = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardInstanceDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardInstanceDatas_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardTextElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Side = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    ElementId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TagType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TagLabel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DefaultContent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTextElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardTextElements_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TextTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ElementId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CustomLabel = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextTags_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BluetoothAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    OriginalAddress = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CurrentCardId = table.Column<int>(type: "INTEGER", nullable: true),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastConnected = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CustomIndex = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Cards_CurrentCardId",
                        column: x => x.CurrentCardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Devices_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GroupCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupCards_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "general"),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackgroundImages_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ElementImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true, defaultValue: "general"),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementImages_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailA = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailB = table.Column<string>(type: "TEXT", nullable: true),
                    LayoutDataA = table.Column<string>(type: "TEXT", nullable: true),
                    LayoutDataB = table.Column<string>(type: "TEXT", nullable: true),
                    Dimensions = table.Column<string>(type: "TEXT", nullable: true),
                    OrganizationId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, defaultValue: "general"),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Templates_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeployHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeployedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DeployedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    IsScheduled = table.Column<bool>(type: "INTEGER", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeployHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeployHistories_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeployHistories_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundImages_Category",
                table: "BackgroundImages",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundImages_CreatedAt",
                table: "BackgroundImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundImages_CreatedBy",
                table: "BackgroundImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundImages_IsActive",
                table: "BackgroundImages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundImages_IsPublic",
                table: "BackgroundImages",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_CardInstanceDatas_CardId_InstanceName_Side_TagType",
                table: "CardInstanceDatas",
                columns: new[] { "CardId", "InstanceName", "Side", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_CardInstanceDatas_TagType",
                table: "CardInstanceDatas",
                column: "TagType");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CreatedAt",
                table: "Cards",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Status",
                table: "Cards",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CardTextElements_CardId_Side_ElementId",
                table: "CardTextElements",
                columns: new[] { "CardId", "Side", "ElementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CardTextElements_TagType",
                table: "CardTextElements",
                column: "TagType");

            migrationBuilder.CreateIndex(
                name: "IX_CustomColors_ColorValue",
                table: "CustomColors",
                column: "ColorValue");

            migrationBuilder.CreateIndex(
                name: "IX_CustomColors_CreatedAt",
                table: "CustomColors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomColors_IsPublic",
                table: "CustomColors",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_DeployHistories_CardId",
                table: "DeployHistories",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_DeployHistories_CreatedAt",
                table: "DeployHistories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DeployHistories_DeviceId",
                table: "DeployHistories",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeployHistories_Status",
                table: "DeployHistories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_BluetoothAddress",
                table: "Devices",
                column: "BluetoothAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CurrentCardId",
                table: "Devices",
                column: "CurrentCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_GroupId",
                table: "Devices",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LastConnected",
                table: "Devices",
                column: "LastConnected");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Status",
                table: "Devices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ElementImages_Category",
                table: "ElementImages",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ElementImages_CreatedAt",
                table: "ElementImages",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ElementImages_CreatedBy",
                table: "ElementImages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ElementImages_IsActive",
                table: "ElementImages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ElementImages_IsPublic",
                table: "ElementImages",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCards_CardId",
                table: "GroupCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCards_GroupId_CardId",
                table: "GroupCards",
                columns: new[] { "GroupId", "CardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedAt",
                table: "Groups",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Name",
                table: "Groups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_Category",
                table: "Templates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedAt",
                table: "Templates",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_CreatedBy",
                table: "Templates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_IsActive",
                table: "Templates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_IsPublic",
                table: "Templates",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_TextTags_CardId",
                table: "TextTags",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_TextTags_CreatedAt",
                table: "TextTags",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TextTags_ElementId",
                table: "TextTags",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_TextTags_TagType",
                table: "TextTags",
                column: "TagType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundImages");

            migrationBuilder.DropTable(
                name: "CardInstanceDatas");

            migrationBuilder.DropTable(
                name: "CardTextElements");

            migrationBuilder.DropTable(
                name: "CustomColors");

            migrationBuilder.DropTable(
                name: "DeployHistories");

            migrationBuilder.DropTable(
                name: "ElementImages");

            migrationBuilder.DropTable(
                name: "GroupCards");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "TextTags");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
