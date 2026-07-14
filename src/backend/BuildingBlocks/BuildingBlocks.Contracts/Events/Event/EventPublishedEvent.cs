namespace BuildingBlocks.Contracts.Events.Event
{
    public class EventPublishedEvent
    {
        public Guid EventId { get; init; }
        public string EventTitle { get; init; } = default!;
        public string EventImage { get; init; } = default!;
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = default!;
        public Guid OrganizerId { get; init; }
        public string OrganizerName { get; init; } = default!;
        public DateTime SaleOpenAt { get; init; }
        public DateTime SaleCloseAt { get; init; }
        public List<ShowtimePublishedDto> Showtimes { get; init; } = new();
        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();
    }

    public class ShowtimePublishedDto
    {
        public Guid ShowTimeId { get; init; }
        public DateTime StartAt { get; init; }
        public DateTime EndAt { get; init; }
        public List<TicketTypePublishedDto> TicketTypes { get; init; } = new();
    }

    public class TicketTypePublishedDto
    {
        public Guid TicketTypeId { get; init; }
        public string TicketTypeName { get; init; } = default!;
        public int Capacity { get; init; }
        public decimal Price { get; init; }
        public bool IsReservingSeat { get; init; }
    }
}
