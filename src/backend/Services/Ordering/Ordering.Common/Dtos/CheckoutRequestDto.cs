namespace Ordering.Common.Dtos
{
    public class CheckoutRequestDto
    {
        public Guid ShowtimeId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public DateTime ShowtimeStartAt { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public List<CheckoutItemDto> Items { get; set; } = new();
    }

    public class CheckoutItemDto
    {
        public Guid? SeatId { get; set; }
        public string? SeatName { get; set; }
        public string? RowName { get; set; }
        public Guid TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
