using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApp.Migrations
{
    /// <inheritdoc />
    public partial class AddClubNameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomDuClub",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NomDuClub",
                table: "AspNetUsers");
        }
    }
}
