using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Interfaces.IRepositories
{
    public interface IUserNotificationRepository : IBaseRepository<UserNotification, NotificationDbContext>
    {
    }
}
