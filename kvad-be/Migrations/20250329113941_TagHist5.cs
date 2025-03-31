using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TagHist5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "TagHists");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagHists",
                table: "TagHists",
                columns: new[] { "TagId", "Timestamp" });
        }
    }
}
