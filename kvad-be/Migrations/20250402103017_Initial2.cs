using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layout_Dashboards_DashboardId",
                table: "Layout");

            migrationBuilder.DropForeignKey(
                name: "FK_Layout_Layout_ParentDashboardId_ParentId",
                table: "Layout");

            migrationBuilder.DropForeignKey(
                name: "FK_Layout_Widget_WidgetId",
                table: "Layout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Layout",
                table: "Layout");

            migrationBuilder.RenameTable(
                name: "Layout",
                newName: "Layouts");

            migrationBuilder.RenameIndex(
                name: "IX_Layout_WidgetId",
                table: "Layouts",
                newName: "IX_Layouts_WidgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Layout_ParentDashboardId_ParentId",
                table: "Layouts",
                newName: "IX_Layouts_ParentDashboardId_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Layout_DashboardId",
                table: "Layouts",
                newName: "IX_Layouts_DashboardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Layouts",
                table: "Layouts",
                columns: new[] { "DashboardId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Dashboards_DashboardId",
                table: "Layouts",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Layouts_ParentDashboardId_ParentId",
                table: "Layouts",
                columns: new[] { "ParentDashboardId", "ParentId" },
                principalTable: "Layouts",
                principalColumns: new[] { "DashboardId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Widget_WidgetId",
                table: "Layouts",
                column: "WidgetId",
                principalTable: "Widget",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Dashboards_DashboardId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Layouts_ParentDashboardId_ParentId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Widget_WidgetId",
                table: "Layouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Layouts",
                table: "Layouts");

            migrationBuilder.RenameTable(
                name: "Layouts",
                newName: "Layout");

            migrationBuilder.RenameIndex(
                name: "IX_Layouts_WidgetId",
                table: "Layout",
                newName: "IX_Layout_WidgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Layouts_ParentDashboardId_ParentId",
                table: "Layout",
                newName: "IX_Layout_ParentDashboardId_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Layouts_DashboardId",
                table: "Layout",
                newName: "IX_Layout_DashboardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Layout",
                table: "Layout",
                columns: new[] { "DashboardId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Layout_Dashboards_DashboardId",
                table: "Layout",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Layout_Layout_ParentDashboardId_ParentId",
                table: "Layout",
                columns: new[] { "ParentDashboardId", "ParentId" },
                principalTable: "Layout",
                principalColumns: new[] { "DashboardId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Layout_Widget_WidgetId",
                table: "Layout",
                column: "WidgetId",
                principalTable: "Widget",
                principalColumn: "Id");
        }
    }
}
