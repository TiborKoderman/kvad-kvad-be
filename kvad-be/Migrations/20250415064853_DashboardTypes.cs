using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class DashboardTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Widget_WidgetId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Widget_Dashboards_DashboardId",
                table: "Widget");

            migrationBuilder.DropForeignKey(
                name: "FK_Widget_WidgetType_TypeId",
                table: "Widget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WidgetType",
                table: "WidgetType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Widget",
                table: "Widget");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Dashboards");

            migrationBuilder.RenameTable(
                name: "WidgetType",
                newName: "WidgetTypes");

            migrationBuilder.RenameTable(
                name: "Widget",
                newName: "Widgets");

            migrationBuilder.RenameIndex(
                name: "IX_Widget_TypeId",
                table: "Widgets",
                newName: "IX_Widgets_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Widget_DashboardId",
                table: "Widgets",
                newName: "IX_Widgets_DashboardId");

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "Dashboards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WidgetTypes",
                table: "WidgetTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Widgets",
                table: "Widgets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DashboardTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    VueComponent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DashboardTypes",
                columns: new[] { "Id", "Description", "Name", "VueComponent" },
                values: new object[,]
                {
                    { "custom", "Custom dashboard type", "Custom", "CustomDashboard.vue" },
                    { "grid", "Grid dashboard type", "Grid", "GridDashboard.vue" },
                    { "masonry", "Masonry dashboard type", "Masonry", "MasonryDashboard.vue" },
                    { "scada", "Scada dashboard type", "Scada", "ScadaDashboard.vue" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Widgets_WidgetId",
                table: "Layouts",
                column: "WidgetId",
                principalTable: "Widgets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Widgets_Dashboards_DashboardId",
                table: "Widgets",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Widgets_WidgetTypes_TypeId",
                table: "Widgets",
                column: "TypeId",
                principalTable: "WidgetTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Widgets_WidgetId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Widgets_Dashboards_DashboardId",
                table: "Widgets");

            migrationBuilder.DropForeignKey(
                name: "FK_Widgets_WidgetTypes_TypeId",
                table: "Widgets");

            migrationBuilder.DropTable(
                name: "DashboardTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WidgetTypes",
                table: "WidgetTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Widgets",
                table: "Widgets");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Dashboards");

            migrationBuilder.RenameTable(
                name: "WidgetTypes",
                newName: "WidgetType");

            migrationBuilder.RenameTable(
                name: "Widgets",
                newName: "Widget");

            migrationBuilder.RenameIndex(
                name: "IX_Widgets_TypeId",
                table: "Widget",
                newName: "IX_Widget_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Widgets_DashboardId",
                table: "Widget",
                newName: "IX_Widget_DashboardId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WidgetType",
                table: "WidgetType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Widget",
                table: "Widget",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Widget_WidgetId",
                table: "Layouts",
                column: "WidgetId",
                principalTable: "Widget",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Widget_Dashboards_DashboardId",
                table: "Widget",
                column: "DashboardId",
                principalTable: "Dashboards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Widget_WidgetType_TypeId",
                table: "Widget",
                column: "TypeId",
                principalTable: "WidgetType",
                principalColumn: "Id");
        }
    }
}
