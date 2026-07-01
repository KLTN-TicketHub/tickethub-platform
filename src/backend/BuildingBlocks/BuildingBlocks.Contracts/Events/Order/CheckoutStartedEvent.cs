namespace BuildingBlocks.Contracts.Events.Order
{
    public class CheckoutStartedEvent
    {
        public Guid OrderId { get; init; }
        public Guid UserId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public string CustomerPhone { get; init; } = string.Empty;
        public Guid ShowtimeId { get; init; }
        public decimal TotalPrice { get; init; }
        public string PaymentMethod { get; init; } = default!;
        public List<CheckoutStartedItemDto> Items { get; init; } = new();
    }

    public class CheckoutStartedItemDto
    {
        public Guid? SeatId { get; init; }
        public Guid TicketTypeId { get; init; }
        public int Quantity { get; init; }
    }
}
