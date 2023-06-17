using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class OneToManyFolderNotebook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notebooks_Folders_OwnerId",
                table: "Notebooks");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Notebooks",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Notebooks_ParentId",
                table: "Notebooks",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notebooks_Folders_ParentId",
                table: "Notebooks",
                column: "ParentId",
                principalTable: "Folders",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notebooks_Folders_ParentId",
                table: "Notebooks");

            migrationBuilder.DropIndex(
                name: "IX_Notebooks_ParentId",
                table: "Notebooks");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Notebooks");

            migrationBuilder.AddForeignKey(
                name: "FK_Notebooks_Folders_OwnerId",
                table: "Notebooks",
                column: "OwnerId",
                principalTable: "Folders",
                principalColumn: "FolderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
