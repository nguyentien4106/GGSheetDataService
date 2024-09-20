using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editnotification1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Action",
                table: "Notifications",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Type",
                table: "Notifications",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
