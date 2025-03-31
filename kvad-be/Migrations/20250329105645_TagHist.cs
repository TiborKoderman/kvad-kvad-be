using System;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagHist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagHists",
                columns: table => new
                {
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false),
                    TagDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId1 = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<JsonValue>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagHists", x => new { x.TagId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_TagHists_Tags_TagDeviceId_TagId1",
                        columns: x => new { x.TagDeviceId, x.TagId1 },
                        principalTable: "Tags",
                        principalColumns: new[] { "DeviceId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagHists_TagDeviceId_TagId1",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId1" });

            migrationBuilder.CreateIndex(
                name: "IX_TagHists_TagId_Timestamp",
                table: "TagHists",
                columns: new[] { "TagId", "Timestamp" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagHists");
        }
    }
}
