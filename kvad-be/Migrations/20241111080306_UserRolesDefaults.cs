using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class UserRolesDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "icon",
                table: "Users",
                newName: "Icon");

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "UserUserRole",
                columns: new[] { "UserRolesId", "UsersId" },
                values: new object[] { 1, new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserUserRole",
                keyColumns: new[] { "UserRolesId", "UsersId" },
                keyValues: new object[] { 1, new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "Users",
                newName: "icon");
        }
    }
}
