using BuildingBlocks.Infrastructure.Auditing;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Entities;

namespace Payment.Infrastructure.Data.Contexts
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentDbContext).Assembly);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
