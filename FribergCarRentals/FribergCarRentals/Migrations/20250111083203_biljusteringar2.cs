using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class biljusteringar2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxMotoreffekt",
                table: "Bilar");

            migrationBuilder.RenameColumn(
                name: "ÅrsModell",
                table: "Bilar",
                newName: "Årsmodell");

            migrationBuilder.RenameColumn(
                name: "BränsleFörbrukning",
                table: "Bilar",
                newName: "Bränsleförbrukning");

            migrationBuilder.RenameColumn(
                name: "Tankvolym",
                table: "Bilar",
                newName: "Växellåda");

            migrationBuilder.AddColumn<string>(
                name: "TankvolymRäckvidd",
                table: "Bilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TankvolymRäckvidd",
                table: "Bilar");

            migrationBuilder.RenameColumn(
                name: "Årsmodell",
                table: "Bilar",
                newName: "ÅrsModell");

            migrationBuilder.RenameColumn(
                name: "Bränsleförbrukning",
                table: "Bilar",
                newName: "BränsleFörbrukning");

            migrationBuilder.RenameColumn(
                name: "Växellåda",
                table: "Bilar",
                newName: "Tankvolym");

            migrationBuilder.AddColumn<int>(
                name: "MaxMotoreffekt",
                table: "Bilar",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
