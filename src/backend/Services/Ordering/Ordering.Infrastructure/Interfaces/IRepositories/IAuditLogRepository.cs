using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Ordering.Infrastructure.Data.Contexts;

namespace Ordering.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, OrderingDbContext>
    {
    }
}
