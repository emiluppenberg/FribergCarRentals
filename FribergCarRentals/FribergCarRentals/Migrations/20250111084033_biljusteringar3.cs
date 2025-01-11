using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class biljusteringar3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bränsleförbrukning",
                table: "Bilar");

            migrationBuilder.DropColumn(
                name: "TankvolymRäckvidd",
                table: "Bilar");

            migrationBuilder.RenameColumn(
                name: "Drivlina",
                table: "Bilar",
                newName: "Drivning");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Drivning",
                table: "Bilar",
                newName: "Drivlina");

            migrationBuilder.AddColumn<string>(
                name: "Bränsleförbrukning",
                table: "Bilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TankvolymRäckvidd",
                table: "Bilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
