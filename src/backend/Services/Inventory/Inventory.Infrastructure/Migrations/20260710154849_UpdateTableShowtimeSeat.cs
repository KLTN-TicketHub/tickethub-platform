using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableShowtimeSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedInAt",
                table: "ShowtimeSeat");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "ShowtimeSeat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedInAt",
                table: "ShowtimeSeat",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "ShowtimeSeat",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
