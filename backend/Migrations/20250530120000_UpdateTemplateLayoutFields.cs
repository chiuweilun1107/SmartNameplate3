using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplateLayoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 重命名現有的 LayoutData 欄位為 LayoutDataA
            migrationBuilder.RenameColumn(
                name: "LayoutData",
                table: "Templates",
                newName: "LayoutDataA");

            // 新增 LayoutDataB 欄位
            migrationBuilder.AddColumn<string>(
                name: "LayoutDataB",
                table: "Templates",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 移除 LayoutDataB 欄位
            migrationBuilder.DropColumn(
                name: "LayoutDataB",
                table: "Templates");

            // 重命名 LayoutDataA 回到 LayoutData
            migrationBuilder.RenameColumn(
                name: "LayoutDataA",
                table: "Templates",
                newName: "LayoutData");
        }
    }
}
