using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventImage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OrganizerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SaleOpenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaleCloseAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SnapshotCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSnapshot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShowtimeSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowtimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowtimeSnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShowtimeSnapshot_EventSnapshot_EventSnapshotId",
                        column: x => x.EventSnapshotId,
                        principalTable: "EventSnapshot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketTypeSnapshot",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowtimeSnapshotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketTypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    IsReservingSeat = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTypeSnapshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTypeSnapshot_ShowtimeSnapshot_ShowtimeSnapshotId",
                        column: x => x.ShowtimeSnapshotId,
                        principalTable: "ShowtimeSnapshot",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSnapshot_EventId",
                table: "EventSnapshot",
                column: "EventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSnapshot_EventSnapshotId",
                table: "ShowtimeSnapshot",
                column: "EventSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_ShowtimeSnapshot_ShowtimeId",
                table: "ShowtimeSnapshot",
                column: "ShowtimeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketTypeSnapshot_ShowtimeSnapshotId",
                table: "TicketTypeSnapshot",
                column: "ShowtimeSnapshotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketTypeSnapshot");

            migrationBuilder.DropTable(
                name: "ShowtimeSnapshot");

            migrationBuilder.DropTable(
                name: "EventSnapshot");
        }
    }
}
