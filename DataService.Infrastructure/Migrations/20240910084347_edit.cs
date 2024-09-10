using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets");

            migrationBuilder.AddForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets");

            migrationBuilder.AddForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
