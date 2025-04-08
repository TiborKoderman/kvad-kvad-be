using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Dashboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Widget",
                newName: "Title");

            migrationBuilder.AddColumn<Guid>(
                name: "DashboardId",
                table: "Widget",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Widget_DashboardId",
                table: "Widget",
                column: "DashboardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Widget_Dashboards_DashboardId",
                table: "Widget",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Widget_Dashboards_DashboardId",
                table: "Widget");

            migrationBuilder.DropIndex(
                name: "IX_Widget_DashboardId",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "DashboardId",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Dashboards");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Widget",
                newName: "Name");
        }
    }
}
