namespace BuildingBlocks.Contracts.Commands.Inventory
{
    public class ReserveSeatsCommand
    {
        public Guid OrderId { get; init; }

        public Guid ShowtimeId { get; init; }

        public Guid UserId { get; init; }

        public List<ReserveSeatItemDto> Seats { get; init; } = new List<ReserveSeatItemDto>();

        public Guid? TicketTypeId { get; init; }

        public int Quantity { get; init; }
    }

    public class ReserveSeatItemDto
    {
        public Guid SeatId { get; init; }
        public string Row { get; set; } = string.Empty;
        public string SeatName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
