using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataEntity.Migrations
{
    /// <inheritdoc />
    public partial class _005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Zakaznici",
                columns: table => new
                {
                    ID_Zakaznik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jmeno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prijmeni = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatumNarozeni = table.Column<DateTime>(type: "date", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PocetPronajmu = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zakaznici", x => x.ID_Zakaznik);
                });

            migrationBuilder.CreateTable(
                name: "Pronajmy",
                columns: table => new
                {
                    ID_Pronajem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Auto = table.Column<int>(type: "int", nullable: false),
                    ID_Zakaznik = table.Column<int>(type: "int", nullable: false),
                    DatumOd = table.Column<DateTime>(type: "date", nullable: false),
                    DatumDo = table.Column<DateTime>(type: "date", nullable: false),
                    DelkaDni = table.Column<int>(type: "int", nullable: false),
                    CenaCelkem = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pronajmy", x => x.ID_Pronajem);
                    table.ForeignKey(
                        name: "FK_Pronajmy_Vozidla_ID_Auto",
                        column: x => x.ID_Auto,
                        principalTable: "Vozidla",
                        principalColumn: "ID_Auto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pronajmy_Zakaznici_ID_Zakaznik",
                        column: x => x.ID_Zakaznik,
                        principalTable: "Zakaznici",
                        principalColumn: "ID_Zakaznik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pokuty",
                columns: table => new
                {
                    ID_Pokuta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Pronajem = table.Column<int>(type: "int", nullable: false),
                    DatumPokuty = table.Column<DateTime>(type: "date", nullable: false),
                    Castka = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Duvod = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    DatumVytvoreni = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokuty", x => x.ID_Pokuta);
                    table.ForeignKey(
                        name: "FK_Pokuty_Pronajmy_ID_Pronajem",
                        column: x => x.ID_Pronajem,
                        principalTable: "Pronajmy",
                        principalColumn: "ID_Pronajem",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pokuty_ID_Pronajem",
                table: "Pokuty",
                column: "ID_Pronajem");

            migrationBuilder.CreateIndex(
                name: "IX_Pronajmy_ID_Auto",
                table: "Pronajmy",
                column: "ID_Auto");

            migrationBuilder.CreateIndex(
                name: "IX_Pronajmy_ID_Zakaznik",
                table: "Pronajmy",
                column: "ID_Zakaznik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pokuty");

            migrationBuilder.DropTable(
                name: "Pronajmy");

            migrationBuilder.DropTable(
                name: "Zakaznici");
        }
    }
}
