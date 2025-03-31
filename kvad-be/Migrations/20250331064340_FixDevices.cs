using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class FixDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WidgetType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    VueComponent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Widget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widget_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Widget_WidgetType_TypeId",
                        column: x => x.TypeId,
                        principalTable: "WidgetType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Widget_DashboardId",
                table: "Widget",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_TypeId",
                table: "Widget",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Widget");

            migrationBuilder.DropTable(
                name: "WidgetType");
        }
    }
}
