namespace BuildingBlocks.Contracts.Events.Order
{
    public class OrderPaidEvent
    {
        public Guid OrderId { get; init; }

        public Guid UserId { get; init; }

        public decimal TotalPrice { get; init; }

        public string CustomerName { get; init; } = string.Empty;

        public string CustomerEmail { get; init; } = string.Empty;

        public string CustomerPhone { get; init; } = string.Empty;

        public Guid ShowTimeId { get; init; }

        public Guid EventId { get; init; }

        public string EventTitle { get; init; } = string.Empty;

        public Guid CategoryId { get; init; }

        public string CategoryName { get; init; } = string.Empty;

        public DateTime ShowtimeStartAt { get; init; }

        public Guid OrganizerId { get; init; }
        public string OrganizerName { get; init; } = string.Empty;
        public string EventImage { get; init; } = string.Empty;

        public DateTime ShowtimeEndAt { get; init; }
        public List<OrderPaidItemDto> Items { get; init; } = new();

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }
    public class OrderPaidItemDto
    {
        public Guid? SeatId { get; init; }

        public string? SeatName { get; init; }

        public string? RowName { get; init; }

        public Guid TicketTypeId { get; init; }

        public string TicketTypeName { get; init; } = string.Empty;

        public decimal Price { get; init; }

        public int Quantity { get; init; }
    }
}
