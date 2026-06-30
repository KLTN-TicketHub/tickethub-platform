namespace BuildingBlocks.Contracts.Commands.Order
{
    public class CancelOrderCommand
    {
        public Guid OrderId { get; init; }

        public string Reason { get; init; } = string.Empty;
    }
}
