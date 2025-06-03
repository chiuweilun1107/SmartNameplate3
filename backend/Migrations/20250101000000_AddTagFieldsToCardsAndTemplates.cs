using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTagFieldsToCardsAndTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Cards",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Templates",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Templates");
        }
    }
} 