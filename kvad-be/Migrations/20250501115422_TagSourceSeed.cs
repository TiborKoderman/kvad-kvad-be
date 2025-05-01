using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagSourceSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TagSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSources", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TagSources",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Constant" },
                    { 2, "Computed" },
                    { 3, "MQTT" },
                    { 4, "Modbus" },
                    { 5, "OPC UA" },
                    { 6, "HTTP" },
                    { 7, "WebSocket" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SourceId",
                table: "Tags",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_TagSources_SourceId",
                table: "Tags",
                column: "SourceId",
                principalTable: "TagSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_TagSources_SourceId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "TagSources");

            migrationBuilder.DropIndex(
                name: "IX_Tags_SourceId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Tags");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
