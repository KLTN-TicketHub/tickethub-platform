using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, NotificationDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(NotificationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
