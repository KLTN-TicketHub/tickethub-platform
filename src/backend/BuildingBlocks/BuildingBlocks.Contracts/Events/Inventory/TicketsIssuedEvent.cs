namespace BuildingBlocks.Contracts.Events.Inventory
{
    public class TicketsIssuedEvent
    {
        public Guid OrderId { get; init; }
        public Guid ShowTimeId { get; init; }
        public Guid EventId { get; init; }
        public string EventTitle { get; init; } = string.Empty;
        public DateTime ShowtimeStartAt { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public List<IssuedTicketDto> Tickets { get; init; } = new();
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }

    public class IssuedTicketDto
    {
        public Guid IssuedTicketId { get; init; }
        public string QrCodeToken { get; init; } = string.Empty;
        public string QrCodeBase64 { get; init; } = string.Empty;
        public string? SeatName { get; init; }
        public string? RowName { get; init; }
        public string TicketTypeName { get; init; } = string.Empty;
        public decimal Price { get; init; }
    }
}
