namespace BuildingBlocks.Contracts.Commands.Inventory
{
    public class ReserveSeatsCommand
    {
        public Guid OrderId { get; init; }

        public Guid ShowtimeId { get; init; }

        public Guid UserId { get; init; }

        public List<Guid> SeatIds { get; init; } = new List<Guid>();

        public Guid? TicketTypeId { get; init; }

        public int Quantity { get; init; }
    }
}
