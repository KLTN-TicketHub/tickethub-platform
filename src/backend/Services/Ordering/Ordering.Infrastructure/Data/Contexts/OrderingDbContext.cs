using BuildingBlocks.Domain.Outbox;
using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Data.Contexts
{
    public class OrderingDbContext : DbContext
    {
        public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderBookingState> OrderBookingStates { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        }
    }
}
