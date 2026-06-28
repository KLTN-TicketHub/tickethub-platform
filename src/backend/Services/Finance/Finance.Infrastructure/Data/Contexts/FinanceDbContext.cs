using BuildingBlocks.Domain.Outbox;
using BuildingBlocks.Infrastructure.Auditing;
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
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }
    }
}
