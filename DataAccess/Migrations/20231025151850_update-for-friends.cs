using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class updateforfriends : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AspNetUsers_WhiteId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_FirstUserId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_SecondSecondId",
                table: "UserFriends");

            migrationBuilder.RenameColumn(
                name: "SecondSecondId",
                table: "UserFriends",
                newName: "ReceiverUserId");

            migrationBuilder.RenameColumn(
                name: "FirstUserId",
                table: "UserFriends",
                newName: "SenderUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriends_SecondSecondId",
                table: "UserFriends",
                newName: "IX_UserFriends_ReceiverUserId");

            migrationBuilder.RenameColumn(
                name: "WhiteId",
                table: "Sessions",
                newName: "RedId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_WhiteId",
                table: "Sessions",
                newName: "IX_Sessions_RedId");

            migrationBuilder.AddColumn<bool>(
                name: "IsReceived",
                table: "UserFriends",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AspNetUsers_RedId",
                table: "Sessions",
                column: "RedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverUserId",
                table: "UserFriends",
                column: "ReceiverUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_SenderUserId",
                table: "UserFriends",
                column: "SenderUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AspNetUsers_RedId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_ReceiverUserId",
                table: "UserFriends");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFriends_AspNetUsers_SenderUserId",
                table: "UserFriends");

            migrationBuilder.DropColumn(
                name: "IsReceived",
                table: "UserFriends");

            migrationBuilder.RenameColumn(
                name: "ReceiverUserId",
                table: "UserFriends",
                newName: "SecondSecondId");

            migrationBuilder.RenameColumn(
                name: "SenderUserId",
                table: "UserFriends",
                newName: "FirstUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserFriends_ReceiverUserId",
                table: "UserFriends",
                newName: "IX_UserFriends_SecondSecondId");

            migrationBuilder.RenameColumn(
                name: "RedId",
                table: "Sessions",
                newName: "WhiteId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_RedId",
                table: "Sessions",
                newName: "IX_Sessions_WhiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AspNetUsers_WhiteId",
                table: "Sessions",
                column: "WhiteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_FirstUserId",
                table: "UserFriends",
                column: "FirstUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFriends_AspNetUsers_SecondSecondId",
                table: "UserFriends",
                column: "SecondSecondId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
