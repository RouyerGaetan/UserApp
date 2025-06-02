using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApp.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteOnNoteEvenement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotesEvenements_Evenements_EvenementId",
                table: "NotesEvenements");

            migrationBuilder.AlterColumn<string>(
                name: "Commentaire",
                table: "NotesEvenements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NotesEvenements_Evenements_EvenementId",
                table: "NotesEvenements",
                column: "EvenementId",
                principalTable: "Evenements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotesEvenements_Evenements_EvenementId",
                table: "NotesEvenements");

            migrationBuilder.AlterColumn<string>(
                name: "Commentaire",
                table: "NotesEvenements",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NotesEvenements_Evenements_EvenementId",
                table: "NotesEvenements",
                column: "EvenementId",
                principalTable: "Evenements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
