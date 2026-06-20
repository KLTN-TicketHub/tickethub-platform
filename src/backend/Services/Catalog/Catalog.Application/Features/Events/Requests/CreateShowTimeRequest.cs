namespace Catalog.Application.Features.Events.Requests
{
    public class CreateShowTimeRequest
    {
        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public List<CreateTicketTypeRequest> TicketTypes { get; set; } = new();
    }
}
