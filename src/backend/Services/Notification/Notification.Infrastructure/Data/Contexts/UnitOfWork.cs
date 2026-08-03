using BuildingBlocks.Infrastructure.Data;
using Notification.Infrastructure.Interfaces;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<NotificationDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            NotificationDbContext dbContext,
            IAuditLogRepository auditLogRepository,
            IUserNotificationRepository userNotificationRepository,
            IUserNotificationReadRepository userNotificationReadRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
            UserNotificationRepository = userNotificationRepository;
            UserNotificationReadRepository = userNotificationReadRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
        public IUserNotificationRepository UserNotificationRepository { get; }
        public IUserNotificationReadRepository UserNotificationReadRepository { get; }
    }
}
