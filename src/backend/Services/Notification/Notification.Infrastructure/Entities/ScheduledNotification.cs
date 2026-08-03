using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Enums;

namespace Notification.Infrastructure.Entities
{
    public class ScheduledNotification : BaseEntity, IAggregateRoot
    {
        public Guid? RecipientUserId { get; private set; }

        public string? TargetRole { get; private set; }

        public NotificationType Type { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public string? LinkUrl { get; private set; }

        public DateTime ScheduledAt { get; private set; }

        public ScheduledNotificationStatus Status { get; private set; }

        public DateTime? SentAt { get; private set; }

        public Guid? CreatedNotificationId { get; private set; }

        public ScheduledNotification(
            Guid? recipientUserId,
            string? targetRole,
            NotificationType type,
            string title,
            string message,
            string? linkUrl,
            DateTime scheduledAt)
        {
            Id = Guid.NewGuid();
            RecipientUserId = recipientUserId;
            TargetRole = targetRole;
            Type = type;
            Title = title;
            Message = message;
            LinkUrl = linkUrl;
            ScheduledAt = scheduledAt;
            Status = ScheduledNotificationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsSent(Guid createdNotificationId)
        {
            Status = ScheduledNotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
            CreatedNotificationId = createdNotificationId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            Status = ScheduledNotificationStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
