using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class CascadingDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards",
                column: "CategoryId",
                principalTable: "FlashcardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardCategories_CategoryId",
                table: "Flashcards",
                column: "CategoryId",
                principalTable: "FlashcardCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
