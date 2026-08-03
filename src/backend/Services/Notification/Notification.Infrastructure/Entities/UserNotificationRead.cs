using BuildingBlocks.Domain.DDD;

namespace Notification.Infrastructure.Entities
{
    public class UserNotificationRead : BaseEntity, IAggregateRoot
    {
        public Guid NotificationId { get; private set; }

        public UserNotification Notification { get; private set; } = default!;

        public Guid UserId { get; private set; }

        public DateTime ReadAt { get; private set; }

        public UserNotificationRead(Guid notificationId, Guid userId)
        {
            Id = Guid.NewGuid();
            NotificationId = notificationId;
            UserId = userId;
            ReadAt = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
