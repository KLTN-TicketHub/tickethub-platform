namespace Notification.Common.Dtos.Notifications
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? LinkUrl { get; set; }

        public Guid? ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
