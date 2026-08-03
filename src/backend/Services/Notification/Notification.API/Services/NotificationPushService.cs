using Microsoft.AspNetCore.SignalR;
using Notification.API.Hubs;
using Notification.Common.Dtos.Notifications;
using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.API.Services
{
    public class NotificationPushService : INotificationPushService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationPushService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushAsync(
            Guid? recipientUserId,
            string? targetRole,
            NotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            IClientProxy target = ResolveTarget(recipientUserId, targetRole);

            await target.SendAsync("ReceiveNotification", notification, cancellationToken);
        }

        private IClientProxy ResolveTarget(Guid? recipientUserId, string? targetRole)
        {
            if (recipientUserId.HasValue)
            {
                return _hubContext.Clients.Group($"user_{recipientUserId.Value}");
            }

            if (!string.IsNullOrEmpty(targetRole))
            {
                return _hubContext.Clients.Group($"role_{targetRole}");
            }

            return _hubContext.Clients.All;
        }
    }
}
