using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencijaVozila.Data.Migrations
{
    /// <inheritdoc />
    public partial class OsigurajJedanAktivanNalogPoVozilu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleOrders_VehicleId",
                table: "VehicleOrders");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOrders_ActiveVehicle",
                table: "VehicleOrders",
                column: "VehicleId",
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleOrders_ActiveVehicle",
                table: "VehicleOrders");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOrders_VehicleId",
                table: "VehicleOrders",
                column: "VehicleId");
        }
    }
}
