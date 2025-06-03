using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Devices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "GroupCards",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "GroupCards",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cards",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
