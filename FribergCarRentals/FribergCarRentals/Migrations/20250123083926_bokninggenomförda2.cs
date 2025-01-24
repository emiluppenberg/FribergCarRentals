using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class bokninggenomförda2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Kunder_KundId",
                table: "Bokningar");

            migrationBuilder.DropTable(
                name: "BokningarGenomförda");

            migrationBuilder.AlterColumn<int>(
                name: "KundId",
                table: "Bokningar",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Kunder_KundId",
                table: "Bokningar",
                column: "KundId",
                principalTable: "Kunder",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bokningar_Kunder_KundId",
                table: "Bokningar");

            migrationBuilder.AlterColumn<int>(
                name: "KundId",
                table: "Bokningar",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "BokningarGenomförda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilId = table.Column<int>(type: "int", nullable: false),
                    KundId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Slutdatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Startdatum = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Bokningar_Kunder_KundId",
                table: "Bokningar",
                column: "KundId",
                principalTable: "Kunder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
