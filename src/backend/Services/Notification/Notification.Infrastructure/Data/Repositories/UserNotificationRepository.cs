using BuildingBlocks.Infrastructure.Data;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Data.Repositories
{
    public class UserNotificationRepository : BaseRepository<UserNotification, NotificationDbContext>, IUserNotificationRepository
    {
        public UserNotificationRepository(NotificationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
