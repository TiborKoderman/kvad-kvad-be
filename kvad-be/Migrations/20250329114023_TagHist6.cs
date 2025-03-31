using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagHist6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId",
                table: "TagHists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists");

            migrationBuilder.DropIndex(
                name: "IX_TagHists_TagDeviceId_TagId",
                table: "TagHists");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "TagHists");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagDeviceId",
                table: "TagHists",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId", "Timestamp" });

            migrationBuilder.AddForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId" },
                principalTable: "Tags",
                principalColumns: new[] { "DeviceId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId",
                table: "TagHists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagDeviceId",
                table: "TagHists",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "TagHists",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists",
                columns: new[] { "DeviceId", "TagId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_TagHists_TagDeviceId_TagId",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId" },
                principalTable: "Tags",
                principalColumns: new[] { "DeviceId", "Id" });
        }
    }
}
