using System;
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
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"),
                column: "Icon",
                value: "data/user_icons/cf960f59-cf1f-49cc-8b2c-de4c5e437730.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"),
                column: "Icon",
                value: "cf960f59-cf1f-49cc-8b2c-de4c5e437730.png");
        }
    }
}
