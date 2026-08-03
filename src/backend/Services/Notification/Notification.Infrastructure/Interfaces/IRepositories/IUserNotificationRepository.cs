using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Data.Contexts;
using Notification.Infrastructure.Entities;

namespace Notification.Infrastructure.Interfaces.IRepositories
{
    public interface IUserNotificationRepository : IBaseRepository<UserNotification, NotificationDbContext>
    {
        Task<NotificationOverviewStats> GetOverviewStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default);

        Task<IEnumerable<NotificationTypeStats>> GetTypeStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default);

        Task<IEnumerable<NotificationDailyStats>> GetDailyStatsAsync(
            DateTime fromUtc,
            DateTime toUtc,
            CancellationToken cancellation = default);

        Task<NotificationDetailStats?> GetDetailStatsAsync(
            Guid notificationId,
            CancellationToken cancellation = default);
    }

    public class NotificationOverviewStats
    {
        public int TotalSent { get; set; }
        public int DirectSent { get; set; }
        public int DirectRead { get; set; }
        public int BroadcastSent { get; set; }
        public int BroadcastReadTotal { get; set; }
    }

    public class NotificationTypeStats
    {
        public string Type { get; set; } = string.Empty;
        public int Sent { get; set; }
        public int ReadCount { get; set; }
    }

    public class NotificationDailyStats
    {
        public DateTime Date { get; set; }
        public int Sent { get; set; }
        public int ReadCount { get; set; }
    }

    public class NotificationDetailStats
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? RecipientUserId { get; set; }
        public string? TargetRole { get; set; }
        public bool IsBroadcast { get; set; }
        public int ReadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? FirstReadAt { get; set; }
        public DateTime? LastReadAt { get; set; }
    }
}
