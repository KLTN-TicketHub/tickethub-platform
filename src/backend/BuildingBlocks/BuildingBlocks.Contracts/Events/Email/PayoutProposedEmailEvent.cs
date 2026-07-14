namespace BuildingBlocks.Contracts.Events.Email
{
    public class PayoutProposedEmailEvent
    {
        public Guid PayoutId { get; init; }

        public Guid EventId { get; init; }

        public string EventTitle { get; init; } = string.Empty;

        public string OrganizerEmail { get; init; } = string.Empty;

        public string OrganizerName { get; init; } = string.Empty;

        public decimal GrossAmount { get; init; }

        public decimal AppliedRate { get; init; }

        public decimal FeeAmount { get; init; }

        public decimal NetAmount { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "PayoutProposed";
    }
}
