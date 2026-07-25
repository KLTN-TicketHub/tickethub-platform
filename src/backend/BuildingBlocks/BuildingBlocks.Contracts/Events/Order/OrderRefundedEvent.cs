namespace BuildingBlocks.Contracts.Events.Order
{
    public class OrderRefundedEvent
    {
        public Guid OrderId { get; init; }

        public Guid EventId { get; init; }

        public decimal RefundedAmount { get; init; }

        public DateTime RefundedAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
