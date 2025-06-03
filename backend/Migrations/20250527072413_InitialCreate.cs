using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<JsonElement>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BluetoothAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CurrentCardId = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    LastConnected = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "DeployHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeviceId = table.Column<int>(type: "integer", nullable: false),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeployedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DeployedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsScheduled = table.Column<bool>(type: "boolean", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "Content", "CreatedAt", "Description", "Name", "Status", "Thumbnail", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "主要會議室桌牌", "會議室 A", "Active", null, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, null, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "前台接待桌牌", "接待處", "Inactive", null, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "#007ACC", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "所有會議室的桌牌", "會議室群組", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "#43E97B", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "接待相關的桌牌", "接待區群組", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Devices",
                columns: new[] { "Id", "BluetoothAddress", "CreatedAt", "CurrentCardId", "GroupId", "LastConnected", "Name", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "AA:BB:CC:DD:EE:01", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "會議室A-桌牌", "Connected", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "AA:BB:CC:DD:EE:02", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2, new DateTime(2025, 5, 26, 23, 55, 0, 0, DateTimeKind.Utc), "接待處-桌牌", "Connected", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "AA:BB:CC:DD:EE:03", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, new DateTime(2025, 5, 26, 22, 0, 0, 0, DateTimeKind.Utc), "會議室B-桌牌", "Disconnected", new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "GroupCards",
                columns: new[] { "Id", "CardId", "CreatedAt", "GroupId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, 2, new DateTime(2025, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CreatedAt",
                table: "Cards",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_Status",
                table: "Cards",
                column: "Status");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeployHistories");

            migrationBuilder.DropTable(
                name: "GroupCards");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
