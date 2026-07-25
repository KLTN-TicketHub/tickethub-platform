namespace BuildingBlocks.Contracts.Events.Order
{
    public class OrderRefundRequestedEvent
    {
        public Guid OrderId { get; init; }

        public Guid EventId { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
