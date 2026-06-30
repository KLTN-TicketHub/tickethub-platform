namespace BuildingBlocks.Contracts.Commands.Payment
{
    public class GeneratePaymentLinkCommand
    {
        public Guid OrderId { get; init; }

        public decimal Amount { get; init; }

        public string Gateway { get; init; } = string.Empty;

        public string CustomerName { get; init; } = string.Empty;

        public string CustomerEmail { get; init; } = string.Empty;
    }
}
