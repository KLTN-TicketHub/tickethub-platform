namespace BuildingBlocks.Contracts.Events.Payment
{
    public class PaymentRefundedEvent
    {
        public Guid OrderId { get; init; }

        public string VnpayRefundTransactionId { get; init; } = string.Empty;

        public decimal RefundedAmount { get; init; }

        public DateTime RefundedAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
