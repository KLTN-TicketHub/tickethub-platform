using BuildingBlocks.Domain.Outbox;
using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;

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
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        }
    }
}
