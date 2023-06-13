using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class FolderRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders");

            migrationBuilder.AddColumn<Guid>(
                name: "FolderId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders",
                column: "OwnerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_OwnerId",
                table: "Folders",
                column: "OwnerId");
        }
    }
}
