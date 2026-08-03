using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Models.Pagination;
using Notification.Common.Dtos.Notifications;

namespace Notification.Infrastructure.Interfaces.IServices
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateAsync(
            NotificationRequestedEvent request,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<NotificationDto>> GetInboxAsync(
            Guid userId,
            IEnumerable<string> roles,
            bool onlyUnread,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(
            Guid userId,
            IEnumerable<string> roles,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<PaginatedResult<SentNotificationDto>> GetSentAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
