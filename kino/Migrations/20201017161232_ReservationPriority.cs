using Microsoft.EntityFrameworkCore.Migrations;

namespace kino.Migrations
{
    public partial class ReservationPriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetX",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SetY",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatX",
                table: "Reservations",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatY",
                table: "Reservations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SeatX",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "SeatY",
                table: "Reservations");

            migrationBuilder.AddColumn<int>(
                name: "SetX",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetY",
                table: "Reservations",
                type: "int",
                nullable: true);
        }
    }
}
