using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        public async Task<int> GetDistinctReaderCountAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default)
        {
            return await _dbContext.Set<UserNotificationRead>()
                .AsNoTracking()
                .Where(r => r.Notification.CreatedAt >= fromUtc && r.Notification.CreatedAt < toUtc)
                .Select(r => r.UserId)
                .Distinct()
                .CountAsync(cancellation);
        }
    }
}
