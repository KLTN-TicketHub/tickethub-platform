using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Finance.Infrastructure.Data.Contexts;

namespace Finance.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, FinanceDbContext>
    {
    }
}
