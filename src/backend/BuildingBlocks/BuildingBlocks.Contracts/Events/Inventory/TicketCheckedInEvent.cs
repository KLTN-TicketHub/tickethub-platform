namespace BuildingBlocks.Contracts.Events.Inventory
{
    public class TicketCheckedInEvent
    {
        public Guid IssuedTicketId { get; init; }
        public Guid EventId { get; init; }
        public Guid OrderId { get; init; }
        public Guid UserId { get; init; }
        public DateTime CheckedInAt { get; init; }
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
