using BuildingBlocks.Infrastructure.Auditing;
using MassTransit;
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
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderBookingState> OrderBookingStates { get; set; }
        public DbSet<EventSnapshot> EventSnapshots { get; set; }
        public DbSet<ShowtimeSnapshot> ShowtimeSnapshots { get; set; }
        public DbSet<TicketTypeSnapshot> TicketTypeSnapshots { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
