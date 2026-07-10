namespace Ordering.Common.Dtos
{
    public class CheckoutRequestDto
    {
        public Guid ShowtimeId { get; set; }
        public Guid EventId { get; set; }

        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;

        public List<CheckoutItemDto> Items { get; set; } = new();
    }
    public class CheckoutItemDto
    {
        public Guid? SeatId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Quantity { get; set; }
    }
}
