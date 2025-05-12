using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserApp.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatsToEvenement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Evenements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "Evenements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Evenements");

            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "Evenements");
        }
    }
}
