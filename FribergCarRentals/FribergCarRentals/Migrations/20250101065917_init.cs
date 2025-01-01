using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lösenord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tillverkare = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TillverkningsÅr = table.Column<int>(type: "int", nullable: false),
                    Modell = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drivlina = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bränsle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BränsleFörbrukning = table.Column<int>(type: "int", nullable: false),
                    Tankvolym = table.Column<int>(type: "int", nullable: false),
                    MaxMotoreffekt = table.Column<int>(type: "int", nullable: false),
                    Beskrivning = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kunder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lösenord = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Förnamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Efternamn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonNummer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Postkod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ort = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kunder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bild",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BildPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bild", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bild_Bilar_BilId",
                        column: x => x.BilId,
                        principalTable: "Bilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bokningar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Startdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slutdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KundId = table.Column<int>(type: "int", nullable: false),
                    BilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bokningar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bokningar_Bilar_BilId",
                        column: x => x.BilId,
                        principalTable: "Bilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bokningar_Kunder_KundId",
                        column: x => x.KundId,
                        principalTable: "Kunder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bild_BilId",
                table: "Bild",
                column: "BilId");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_BilId",
                table: "Bokningar",
                column: "BilId");

            migrationBuilder.CreateIndex(
                name: "IX_Bokningar_KundId",
                table: "Bokningar",
                column: "KundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Bild");

            migrationBuilder.DropTable(
                name: "Bokningar");

            migrationBuilder.DropTable(
                name: "Bilar");

            migrationBuilder.DropTable(
                name: "Kunder");
        }
    }
}
