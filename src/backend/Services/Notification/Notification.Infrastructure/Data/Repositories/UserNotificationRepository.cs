using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        public async Task<NotificationOverviewStats> GetOverviewStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default)
        {
            NotificationOverviewStats? stats = await BuildRangeQuery(fromUtc, toUtc)
                .GroupBy(n => 1)
                .Select(g => new NotificationOverviewStats
                {
                    TotalSent = g.Count(),
                    DirectSent = g.Count(n => n.RecipientUserId != null),
                    DirectRead = g.Count(n => n.RecipientUserId != null && n.IsRead),
                    BroadcastSent = g.Count(n => n.RecipientUserId == null),
                    BroadcastReadTotal = g.Sum(n => n.RecipientUserId == null ? n.Reads.Count : 0)
                })
                .FirstOrDefaultAsync(cancellation);

            return stats ?? new NotificationOverviewStats();
        }

        public async Task<IEnumerable<NotificationTypeStats>> GetTypeStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default)
        {
            return await BuildRangeQuery(fromUtc, toUtc)
                .GroupBy(n => n.Type)
                .Select(g => new NotificationTypeStats
                {
                    Type = g.Key.ToString(),
                    Sent = g.Count(),
                    ReadCount = g.Sum(n => n.RecipientUserId != null
                        ? (n.IsRead ? 1 : 0)
                        : n.Reads.Count)
                })
                .OrderByDescending(x => x.Sent)
                .ToListAsync(cancellation);
        }

        public async Task<IEnumerable<NotificationDailyStats>> GetDailyStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default)
        {
            return await BuildRangeQuery(fromUtc, toUtc)
                .GroupBy(n => n.CreatedAt.Date)
                .Select(g => new NotificationDailyStats
                {
                    Date = g.Key,
                    Sent = g.Count(),
                    ReadCount = g.Sum(n => n.RecipientUserId != null
                        ? (n.IsRead ? 1 : 0)
                        : n.Reads.Count)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellation);
        }

        public async Task<NotificationDetailStats?> GetDetailStatsAsync(
            Guid notificationId,
            CancellationToken cancellation = default)
        {
            return await _dbContext.Set<UserNotification>()
                .AsNoTracking()
                .Where(n => n.Id == notificationId)
                .Select(n => new NotificationDetailStats
                {
                    Id = n.Id,
                    Title = n.Title,
                    Type = n.Type.ToString(),
                    RecipientUserId = n.RecipientUserId,
                    TargetRole = n.TargetRole,
                    IsBroadcast = n.RecipientUserId == null,
                    ReadCount = n.RecipientUserId != null
                        ? (n.IsRead ? 1 : 0)
                        : n.Reads.Count,
                    CreatedAt = n.CreatedAt,
                    FirstReadAt = n.RecipientUserId != null
                        ? n.ReadAt
                        : n.Reads.Min(r => (DateTime?)r.ReadAt),
                    LastReadAt = n.RecipientUserId != null
                        ? n.ReadAt
                        : n.Reads.Max(r => (DateTime?)r.ReadAt)
                })
                .FirstOrDefaultAsync(cancellation);
        }

        private IQueryable<UserNotification> BuildRangeQuery(DateTime fromUtc, DateTime toUtc)
        {
            return _dbContext.Set<UserNotification>()
                .AsNoTracking()
                .Where(n => n.CreatedAt >= fromUtc && n.CreatedAt < toUtc);
        }
    }
}
