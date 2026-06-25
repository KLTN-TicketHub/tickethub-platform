namespace BuildingBlocks.Contracts.Events.Event
{
    public class EventPublishedEvent
    {
        public Guid EventId { get; init; }
        public List<ShowtimePublishedDto> Showtimes { get; init; } = new();
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }

    public class ShowtimePublishedDto
    {
        public Guid ShowTimeId { get; init; }
        public List<TicketTypePublishedDto> TicketTypes { get; init; } = new();
    }

    public class TicketTypePublishedDto
    {
        public Guid TicketTypeId { get; init; }
        public int Capacity { get; init; }
        public decimal Price { get; init; }
        public bool IsReservingSeat { get; init; }
    }
}
