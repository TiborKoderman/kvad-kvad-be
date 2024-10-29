using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class DefaultAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Username" },
                values: new object[] { new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"), "$argon2id$v=19$m=32768,t=4,p=1$g8fJIqwvK69pwVZEFI2+NQ$X5P9Sd32U7UTUJmjFP/t6P5vW/7lNS/RQYLE3nPbvXU", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"));
        }
    }
}
