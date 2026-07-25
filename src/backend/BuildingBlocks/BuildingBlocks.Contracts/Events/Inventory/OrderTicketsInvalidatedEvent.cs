namespace BuildingBlocks.Contracts.Events.Inventory
{
    public class OrderTicketsInvalidatedEvent
    {
        public Guid OrderId { get; init; }

        public decimal RefundableAmount { get; init; }

        public int CancelledTicketCount { get; init; }

        public int KeptTicketCount { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
