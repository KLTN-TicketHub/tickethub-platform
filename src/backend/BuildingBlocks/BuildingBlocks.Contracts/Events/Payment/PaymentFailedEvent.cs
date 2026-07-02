namespace BuildingBlocks.Contracts.Events.Payment
{
    public class PaymentFailedEvent
    {
        public Guid OrderId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime CompletedAt { get; init; }
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
        public string Purpose { get; init; } = "PaymentFailed";
    }
}
