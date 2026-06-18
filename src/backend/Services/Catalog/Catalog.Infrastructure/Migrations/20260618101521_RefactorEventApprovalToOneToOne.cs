using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEventApprovalToOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventApproval_EventId",
                table: "EventApproval");

            migrationBuilder.CreateIndex(
                name: "IX_EventApproval_EventId",
                table: "EventApproval",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventApproval_EventId",
                table: "EventApproval");

            migrationBuilder.CreateIndex(
                name: "IX_EventApproval_EventId",
                table: "EventApproval",
                column: "EventId");
        }
    }
}
