namespace Notification.Common.Dtos.Notifications
{
    public class SentNotificationDto
    {
        public Guid Id { get; set; }

        public Guid? RecipientUserId { get; set; }

        public string? TargetRole { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
