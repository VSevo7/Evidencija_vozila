using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EvidencijaVozila.Data.Migrations
{
    /// <inheritdoc />
    public partial class UkloniTireChangeNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TireChangeNote",
                table: "Vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TireChangeNote",
                table: "Vehicles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
