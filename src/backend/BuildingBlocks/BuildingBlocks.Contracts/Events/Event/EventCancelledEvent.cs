namespace BuildingBlocks.Contracts.Events.Event
{
    public class EventCancelledEvent
    {
        public Guid EventId { get; init; }

        public string EventTitle { get; init; } = string.Empty;

        public Guid OrganizerId { get; init; }

        public string OrganizerName { get; init; } = string.Empty;

        public DateTime CancelledAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
