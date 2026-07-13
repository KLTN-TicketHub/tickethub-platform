namespace Ordering.Common.Dtos.Reports
{
    public class EventOrderListItemDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
        public Guid ShowtimeId { get; set; }
        public DateTime ShowtimeStartAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public List<EventOrderItemDto> Items { get; set; } = new();
    }

    public class EventOrderItemDto
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

