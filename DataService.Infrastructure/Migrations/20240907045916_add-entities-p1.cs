using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addentitiesp1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sheet_Devices_DeviceId",
                table: "Sheet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sheet",
                table: "Sheet");

            migrationBuilder.RenameTable(
                name: "Sheet",
                newName: "Sheets");

            migrationBuilder.RenameIndex(
                name: "IX_Sheet_DeviceId",
                table: "Sheets",
                newName: "IX_Sheets_DeviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sheets",
                table: "Sheets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sheets_Devices_DeviceId",
                table: "Sheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sheets",
                table: "Sheets");

            migrationBuilder.RenameTable(
                name: "Sheets",
                newName: "Sheet");

            migrationBuilder.RenameIndex(
                name: "IX_Sheets_DeviceId",
                table: "Sheet",
                newName: "IX_Sheet_DeviceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sheet",
                table: "Sheet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sheet_Devices_DeviceId",
                table: "Sheet",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
