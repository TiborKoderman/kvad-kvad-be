using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class FixDashbordTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DashboardTypeId",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_DashboardTypeId",
                table: "Dashboards",
                column: "DashboardTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dashboards_DashboardTypes_DashboardTypeId",
                table: "Dashboards",
                column: "DashboardTypeId",
                principalTable: "DashboardTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dashboards_DashboardTypes_DashboardTypeId",
                table: "Dashboards");

            migrationBuilder.DropIndex(
                name: "IX_Dashboards_DashboardTypeId",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "DashboardTypeId",
                table: "Dashboards");
        }
    }
}
