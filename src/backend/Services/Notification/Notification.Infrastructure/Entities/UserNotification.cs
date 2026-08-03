using BuildingBlocks.Domain.DDD;
using Notification.Infrastructure.Enums;

namespace Notification.Infrastructure.Entities
{
    public class UserNotification : BaseEntity, IAggregateRoot
    {
        public Guid? RecipientUserId { get; private set; }

        public string? TargetRole { get; private set; }

        public NotificationType Type { get; private set; }

        public string Title { get; private set; }

        public string Message { get; private set; }

        public string? LinkUrl { get; private set; }

        public Guid? ReferenceId { get; private set; }

        public bool IsRead { get; private set; }

        public DateTime? ReadAt { get; private set; }

        private readonly List<UserNotificationRead> _reads = new List<UserNotificationRead>();
        public IReadOnlyCollection<UserNotificationRead> Reads => _reads.AsReadOnly();

        public UserNotification(
            Guid? recipientUserId,
            string? targetRole,
            NotificationType type,
            string title,
            string message,
            string? linkUrl,
            Guid? referenceId)
        {
            Id = Guid.NewGuid();
            RecipientUserId = recipientUserId;
            TargetRole = targetRole;
            Type = type;
            Title = title;
            Message = message;
            LinkUrl = linkUrl;
            ReferenceId = referenceId;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            if (IsRead)
            {
                return;
            }

            IsRead = true;
            ReadAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
