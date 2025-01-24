using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class bokninggenomförda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ÄrAktiv",
                table: "Bilar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "BokningarGenomförda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Startdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slutdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KundId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BokningarGenomförda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BokningarGenomförda_Bilar_BilId",
                        column: x => x.BilId,
                        principalTable: "Bilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BokningarGenomförda_BilId",
                table: "BokningarGenomförda",
                column: "BilId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BokningarGenomförda");

            migrationBuilder.DropColumn(
                name: "ÄrAktiv",
                table: "Bilar");
        }
    }
}
