namespace BuildingBlocks.Contracts.Events.Email
{
    public class OrderRefundedEmailEvent
    {
        public Guid OrderId { get; init; }

        public string EventTitle { get; init; } = string.Empty;

        public string CustomerName { get; init; } = string.Empty;

        public string CustomerEmail { get; init; } = string.Empty;

        public decimal RefundedAmount { get; init; }

        public DateTime RefundedAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "OrderRefunded";
    }
}
