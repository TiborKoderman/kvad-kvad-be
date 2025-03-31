using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagHist4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId1",
                table: "TagHists");

            migrationBuilder.DropIndex(
                name: "IX_TagHists_TagDeviceId_TagId1",
                table: "TagHists");

            migrationBuilder.DropColumn(
                name: "TagId1",
                table: "TagHists");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagDeviceId",
                table: "TagHists",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "TagId",
                table: "TagHists",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId",
                table: "TagHists");

            migrationBuilder.DropIndex(
                name: "IX_TagHists_TagDeviceId_TagId",
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

            migrationBuilder.AlterColumn<int>(
                name: "TagId",
                table: "TagHists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "TagId1",
                table: "TagHists",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TagHists_TagDeviceId_TagId1",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId1" });

            migrationBuilder.AddForeignKey(
                name: "FK_TagHists_Tags_TagDeviceId_TagId1",
                table: "TagHists",
                columns: new[] { "TagDeviceId", "TagId1" },
                principalTable: "Tags",
                principalColumns: new[] { "DeviceId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
