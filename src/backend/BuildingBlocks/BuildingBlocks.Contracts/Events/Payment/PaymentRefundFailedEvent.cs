namespace BuildingBlocks.Contracts.Events.Payment
{
    public class PaymentRefundFailedEvent
    {
        public Guid OrderId { get; init; }

        public string Reason { get; init; } = string.Empty;

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
}
