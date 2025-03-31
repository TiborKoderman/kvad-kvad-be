using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagHistFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_DashboardItems_DashboardItemId_DashboardItemDashboardId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "DashboardItems");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DashboardItemId_DashboardItemDashboardId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DashboardItemDashboardId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DashboardItemId",
                table: "Tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DashboardItemDashboardId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DashboardItemId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DashboardItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DashboardId = table.Column<int>(type: "integer", nullable: false),
                    DashboardId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Config = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardItems", x => new { x.Id, x.DashboardId });
                    table.ForeignKey(
                        name: "FK_DashboardItems_Dashboards_DashboardId1",
                        column: x => x.DashboardId1,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DashboardItemId_DashboardItemDashboardId",
                table: "Tags",
                columns: new[] { "DashboardItemId", "DashboardItemDashboardId" });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardItems_DashboardId1",
                table: "DashboardItems",
                column: "DashboardId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_DashboardItems_DashboardItemId_DashboardItemDashboardId",
                table: "Tags",
                columns: new[] { "DashboardItemId", "DashboardItemDashboardId" },
                principalTable: "DashboardItems",
                principalColumns: new[] { "Id", "DashboardId" });
        }
    }
}
