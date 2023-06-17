using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudySmortAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserImageNowBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfilePicture");

            migrationBuilder.RenameColumn(
                name: "ProfilePictureId",
                table: "Users",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Users",
                newName: "ProfilePictureId");

            migrationBuilder.CreateTable(
                name: "ProfilePicture",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Bytes = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilePicture_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_OwnerId",
                table: "ProfilePicture",
                column: "OwnerId",
                unique: true);
        }
    }
}
