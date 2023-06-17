using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class CompleteFolderRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FolderId",
                table: "Users",
                column: "FolderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Folders_FolderId",
                table: "Users",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Folders_FolderId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FolderId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders",
                column: "OwnerId",
                unique: true);
        }
    }
}
