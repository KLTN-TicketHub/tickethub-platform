namespace BuildingBlocks.Contracts.Events.Payment
{
    public class PaymentLinkGeneratedEvent
    {
        public Guid OrderId { get; init; }
        public string PaymentLink { get; init; } = string.Empty;
        public string MerchantOrderNo { get; init; } = string.Empty;
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
        public string Purpose { get; init; } = "PaymentLinkGenerated";
    }
}
