using BuildingBlocks.Domain.Outbox;
using BuildingBlocks.Infrastructure.Auditing;
using Inventory.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Data.Contexts
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; }
        public DbSet<ShowtimeTicketInventory> ShowtimeTicketInventories { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);
        }
    }
}
