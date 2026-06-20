namespace Catalog.Application.Common.DTOs.Events
{
    public class ShowtimeDto
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public string Status { get; set; }

        public List<TicketTypeDto> TicketTypes { get; set; } = new();
    }
}
