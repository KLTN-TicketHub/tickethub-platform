namespace BuildingBlocks.Contracts.Commands.Inventory
{
    public class InvalidateOrderTicketsCommand
    {
        public Guid OrderId { get; init; }
    }
}
