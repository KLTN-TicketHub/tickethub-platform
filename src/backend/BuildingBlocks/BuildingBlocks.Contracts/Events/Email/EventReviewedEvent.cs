namespace BuildingBlocks.Contracts.Events.Email
{
    public class EventReviewedEvent
    {
        public Guid EventId { get; init; }
        public string EventTitle { get; init; } = string.Empty;
        public string OrganizerEmail { get; init; } = string.Empty;
        public string OrganizerName { get; init; } = string.Empty;
        public bool IsApproved { get; init; }
        public string? Reason { get; init; }
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
        public string Purpose { get; init; } = "EventReviewed";
    }
}
