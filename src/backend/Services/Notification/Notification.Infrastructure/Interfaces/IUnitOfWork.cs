using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        IUserNotificationReadRepository UserNotificationReadRepository { get; }
    }
}
