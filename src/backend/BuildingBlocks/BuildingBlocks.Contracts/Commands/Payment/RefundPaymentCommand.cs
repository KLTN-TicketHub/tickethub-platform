namespace BuildingBlocks.Contracts.Commands.Payment
{
    public class RefundPaymentCommand
    {
        public Guid OrderId { get; init; }

        public decimal Amount { get; init; }

        public string Reason { get; init; } = string.Empty;
    }
}
