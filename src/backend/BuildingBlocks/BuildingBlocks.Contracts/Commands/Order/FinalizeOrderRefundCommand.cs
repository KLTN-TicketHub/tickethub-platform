namespace BuildingBlocks.Contracts.Commands.Order
{
    public class FinalizeOrderRefundCommand
    {
        public Guid OrderId { get; init; }

        public decimal RefundedAmount { get; init; }

        public string VnpayRefundTransactionId { get; init; } = string.Empty;
    }
}
