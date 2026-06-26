using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Domain.DDD;
using Payment.Infrastructure.Data.Contexts;

namespace Payment.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, PaymentDbContext>
    {
    }
}
