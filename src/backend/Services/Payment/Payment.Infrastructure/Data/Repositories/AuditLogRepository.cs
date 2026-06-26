using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Payment.Infrastructure.Data.Contexts;
using Payment.Infrastructure.Interfaces.IRepositories;

namespace Payment.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, PaymentDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(PaymentDbContext dbContext) : base(dbContext)
        {
        }
    }
}
