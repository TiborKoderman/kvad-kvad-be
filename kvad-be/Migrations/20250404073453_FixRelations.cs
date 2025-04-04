using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Dashboards_DashboardId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_DashboardId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DashboardId",
                table: "Groups");

            migrationBuilder.CreateTable(
                name: "DashboardGroup",
                columns: table => new
                {
                    DashboardsId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardGroup", x => new { x.DashboardsId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_DashboardGroup_Dashboards_DashboardsId",
                        column: x => x.DashboardsId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardGroup_GroupsId",
                table: "DashboardGroup",
                column: "GroupsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardGroup");

            migrationBuilder.AddColumn<Guid>(
                name: "DashboardId",
                table: "Groups",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DashboardId",
                table: "Groups",
                column: "DashboardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Dashboards_DashboardId",
                table: "Groups",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id");
        }
    }
}
