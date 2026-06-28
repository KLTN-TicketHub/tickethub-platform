using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Interfaces.IRepositories;

namespace Ordering.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, OrderingDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(OrderingDbContext dbContext) : base(dbContext)
        {
        }
    }
}
