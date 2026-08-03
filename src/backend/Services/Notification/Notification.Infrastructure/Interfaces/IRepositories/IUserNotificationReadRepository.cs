using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Interfaces.IRepositories
{
    public interface IUserNotificationReadRepository : IBaseRepository<UserNotificationRead, NotificationDbContext>
    {
        Task<int> GetDistinctReaderCountAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default);
    }
}
