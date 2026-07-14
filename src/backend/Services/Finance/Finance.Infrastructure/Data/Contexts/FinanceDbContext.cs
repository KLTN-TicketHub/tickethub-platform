using BuildingBlocks.Infrastructure.Auditing;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Contexts
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
        {
        }

        #region DbSet Section
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Entities.CommissionSetting> CommissionSettings { get; set; }
        public DbSet<Entities.EventPayout> EventPayouts { get; set; }
        public DbSet<Entities.OrganizerSnapshot> OrganizerSnapshots { get; set; }
        public DbSet<Entities.PayoutRequest> PayoutRequests { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
