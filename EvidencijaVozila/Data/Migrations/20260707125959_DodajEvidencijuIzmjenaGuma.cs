using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencijaVozila.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodajEvidencijuIzmjenaGuma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleTireChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TireType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MileageAtChange = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTireChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTireChanges_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTireChanges_VehicleId",
                table: "VehicleTireChanges",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleTireChanges");
        }
    }
}
