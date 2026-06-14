namespace Catalog.Application.Features.Events.Requests
{
    public class CreateEventRequest
    {
        public Guid CategoryId { get; set; }

        public Guid? SeatMapId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        public string CoverImageUrl { get; set; }

        public List<CreateTicketTypeRequest> TicketTypes { get; set; }

        public CreateLocationRequest? Location { get; set; }
    }
}
