namespace Notification.Common.Dtos.Notifications
{
    public class SendNotificationRequest
    {
        public Guid? RecipientUserId { get; set; }

        public string? TargetRole { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }
}
