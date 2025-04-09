using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class DeviceVirt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PhysicalAddress>(
                name: "Mac",
                table: "Devices",
                type: "macaddr",
                nullable: true,
                oldClrType: typeof(PhysicalAddress),
                oldType: "macaddr");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<PhysicalAddress>(
                name: "Mac",
                table: "Devices",
                type: "macaddr",
                nullable: false,
                oldClrType: typeof(PhysicalAddress),
                oldType: "macaddr",
                oldNullable: true);
        }
    }
}
