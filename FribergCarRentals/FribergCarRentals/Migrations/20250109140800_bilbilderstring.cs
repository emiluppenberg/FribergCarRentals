using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergCarRentals.Migrations
{
    /// <inheritdoc />
    public partial class bilbilderstring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bild");

            migrationBuilder.AddColumn<string>(
                name: "Bilder",
                table: "Bilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bilder",
                table: "Bilar");

            migrationBuilder.CreateTable(
                name: "Bild",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BilId = table.Column<int>(type: "int", nullable: false),
                    BildPath = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Bild_BilId",
                table: "Bild",
                column: "BilId");
        }
    }
}
