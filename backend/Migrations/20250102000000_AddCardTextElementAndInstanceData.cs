using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNameplate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCardTextElementAndInstanceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardTextElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", 1),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    Side = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    ElementId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TagType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TagLabel = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: ""),
                    DefaultContent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "CardInstanceDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", 1),
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    InstanceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Side = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    TagType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_CardInstanceDatas_CardId_InstanceName_Side_TagType",
                table: "CardInstanceDatas",
                columns: new[] { "CardId", "InstanceName", "Side", "TagType" });

            migrationBuilder.CreateIndex(
                name: "IX_CardInstanceDatas_TagType",
                table: "CardInstanceDatas",
                column: "TagType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardTextElements");

            migrationBuilder.DropTable(
                name: "CardInstanceDatas");
        }
    }
} 