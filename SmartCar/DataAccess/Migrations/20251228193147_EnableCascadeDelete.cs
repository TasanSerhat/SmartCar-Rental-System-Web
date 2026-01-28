using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleId1",
                table: "VehicleImages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId1",
                table: "VehicleImages",
                column: "VehicleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId1",
                table: "VehicleImages",
                column: "VehicleId1",
                principalTable: "Vehicles",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleImages_Vehicles_VehicleId1",
                table: "VehicleImages");

            migrationBuilder.DropIndex(
                name: "IX_VehicleImages_VehicleId1",
                table: "VehicleImages");

            migrationBuilder.DropColumn(
                name: "VehicleId1",
                table: "VehicleImages");
        }
    }
}
