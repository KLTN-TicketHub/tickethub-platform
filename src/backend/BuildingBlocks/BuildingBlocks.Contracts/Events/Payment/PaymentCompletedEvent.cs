namespace BuildingBlocks.Contracts.Events.Payment
{
    public class PaymentCompletedEvent
    {
        public Guid OrderId { get; init; }
        public string TransactionId { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string Gateway { get; init; } = string.Empty;
        public DateTime CompletedAt { get; init; }
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
        public string Purpose { get; init; } = "PaymentCompleted";
    }
}
