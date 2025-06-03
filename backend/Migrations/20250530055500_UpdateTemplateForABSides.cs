using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTemplateForABSides : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LayoutData",
                table: "Templates",
                newName: "LayoutDataB");

            migrationBuilder.AddColumn<string>(
                name: "LayoutDataA",
                table: "Templates",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LayoutDataA",
                table: "Templates");

            migrationBuilder.RenameColumn(
                name: "LayoutDataB",
                table: "Templates",
                newName: "LayoutData");
        }
    }
}
