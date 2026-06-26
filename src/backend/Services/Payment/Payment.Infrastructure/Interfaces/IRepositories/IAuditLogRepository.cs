using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Payment.Infrastructure.Data.Contexts;

namespace Payment.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, PaymentDbContext>
    {
    }
}
