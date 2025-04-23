using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationEvenementUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Evenements",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Evenements_UserId",
                table: "Evenements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evenements_AspNetUsers_UserId",
                table: "Evenements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evenements_AspNetUsers_UserId",
                table: "Evenements");

            migrationBuilder.DropIndex(
                name: "IX_Evenements_UserId",
                table: "Evenements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Evenements");
        }
    }
}
