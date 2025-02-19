using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class UserCanHaveMultipleChatRooms3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomUser_ChatRooms_ChatRoomsId",
                table: "ChatRoomUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomUser_Users_UsersId",
                table: "ChatRoomUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatRoomUser",
                table: "ChatRoomUser");

            migrationBuilder.RenameTable(
                name: "ChatRoomUser",
                newName: "UserChatRooms");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRoomUser_UsersId",
                table: "UserChatRooms",
                newName: "IX_UserChatRooms_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms",
                columns: new[] { "ChatRoomsId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomsId",
                table: "UserChatRooms",
                column: "ChatRoomsId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatRooms_Users_UsersId",
                table: "UserChatRooms",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_ChatRooms_ChatRoomsId",
                table: "UserChatRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChatRooms_Users_UsersId",
                table: "UserChatRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChatRooms",
                table: "UserChatRooms");

            migrationBuilder.RenameTable(
                name: "UserChatRooms",
                newName: "ChatRoomUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserChatRooms_UsersId",
                table: "ChatRoomUser",
                newName: "IX_ChatRoomUser_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatRoomUser",
                table: "ChatRoomUser",
                columns: new[] { "ChatRoomsId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomUser_ChatRooms_ChatRoomsId",
                table: "ChatRoomUser",
                column: "ChatRoomsId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomUser_Users_UsersId",
                table: "ChatRoomUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
