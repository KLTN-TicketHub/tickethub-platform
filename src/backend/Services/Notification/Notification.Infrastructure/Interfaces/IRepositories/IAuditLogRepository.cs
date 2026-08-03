using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Notification.Infrastructure.Data.Contexts;

namespace Notification.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, NotificationDbContext>
    {
    }
}
