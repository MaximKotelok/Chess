using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class rednowiswhite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AspNetUsers_RedId",
                table: "Sessions");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AspNetUsers_WhiteId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "WhiteId",
                table: "Sessions",
                newName: "RedId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_WhiteId",
                table: "Sessions",
                newName: "IX_Sessions_RedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AspNetUsers_RedId",
                table: "Sessions",
                column: "RedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
