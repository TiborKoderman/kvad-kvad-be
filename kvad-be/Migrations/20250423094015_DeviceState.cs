using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class DeviceState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Historicize",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Virtual",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StateDeviceId",
                table: "Devices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Virtual",
                table: "Devices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DeviceState",
                columns: table => new
                {
                    DeviceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    Connected = table.Column<bool>(type: "boolean", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceState", x => x.DeviceId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_StateDeviceId",
                table: "Devices",
                column: "StateDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceState_StateDeviceId",
                table: "Devices",
                column: "StateDeviceId",
                principalTable: "DeviceState",
                principalColumn: "DeviceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceState_StateDeviceId",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "DeviceState");

            migrationBuilder.DropIndex(
                name: "IX_Devices_StateDeviceId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Historicize",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Virtual",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "StateDeviceId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Virtual",
                table: "Devices");
        }
    }
}
