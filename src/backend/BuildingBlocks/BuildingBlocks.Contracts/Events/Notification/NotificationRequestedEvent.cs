namespace BuildingBlocks.Contracts.Events.Notification
{
    public class NotificationRequestedEvent
    {
        public Guid? RecipientUserId { get; init; }

        public string? TargetRole { get; init; }

        public string Type { get; init; } = "General";

        public string Title { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public string? LinkUrl { get; init; }

        public Guid? ReferenceId { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
