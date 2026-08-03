namespace Notification.Common.Dtos.Notifications
{
    public class NotificationDetailStatsDto
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
