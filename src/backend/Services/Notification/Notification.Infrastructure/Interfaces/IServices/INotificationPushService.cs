using Notification.Common.Dtos.Notifications;

namespace Notification.Infrastructure.Interfaces.IServices
{
    public interface INotificationPushService
    {
        Task PushAsync(
            Guid? recipientUserId,
            string? targetRole,
            NotificationDto notification,
            CancellationToken cancellationToken = default);
    }
}
