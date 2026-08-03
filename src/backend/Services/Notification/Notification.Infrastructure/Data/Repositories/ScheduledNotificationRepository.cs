using BuildingBlocks.Infrastructure.Data;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;
using Notification.Infrastructure.Interfaces.IRepositories;

namespace Notification.Infrastructure.Data.Repositories
{
    public class ScheduledNotificationRepository : BaseRepository<ScheduledNotification, NotificationDbContext>, IScheduledNotificationRepository
    {
        public ScheduledNotificationRepository(NotificationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
