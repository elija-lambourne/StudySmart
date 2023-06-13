using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedFlashcardEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardCategory_Users_OwnerId",
                table: "FlashcardCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardCategory_CategoryId",
                table: "Flashcards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashcardCategory",
                table: "FlashcardCategory");

            migrationBuilder.RenameTable(
                name: "FlashcardCategory",
                newName: "FlashcardCategories");

            migrationBuilder.RenameIndex(
                name: "IX_FlashcardCategory_OwnerId",
                table: "FlashcardCategories",
                newName: "IX_FlashcardCategories_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashcardCategories",
                table: "FlashcardCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashcardCategories_Users_OwnerId",
                table: "FlashcardCategories",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards",
                column: "CategoryId",
                principalTable: "FlashcardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashcardCategories_Users_OwnerId",
                table: "FlashcardCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashcardCategories",
                table: "FlashcardCategories");

            migrationBuilder.RenameTable(
                name: "FlashcardCategories",
                newName: "FlashcardCategory");

            migrationBuilder.RenameIndex(
                name: "IX_FlashcardCategories_OwnerId",
                table: "FlashcardCategory",
                newName: "IX_FlashcardCategory_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashcardCategory",
                table: "FlashcardCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashcardCategory_Users_OwnerId",
                table: "FlashcardCategory",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardCategory_CategoryId",
                table: "Flashcards",
                column: "CategoryId",
                principalTable: "FlashcardCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
