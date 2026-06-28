using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, FinanceDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
