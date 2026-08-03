using BuildingBlocks.Infrastructure.Data;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Data.Repositories
{
    public class UserNotificationReadRepository : BaseRepository<UserNotificationRead, NotificationDbContext>, IUserNotificationReadRepository
    {
        public UserNotificationReadRepository(NotificationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
