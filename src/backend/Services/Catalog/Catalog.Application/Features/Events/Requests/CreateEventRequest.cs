namespace Catalog.Application.Features.Events.Requests
{
    public class CreateEventRequest
    {
        public Guid CategoryId { get; set; }

        public Guid? SeatMapId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<CreateShowTimeRequest> ShowTimes { get; set; } = new();

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        public string CoverImageUrl { get; set; }

        public CreateLocationRequest? Location { get; set; }
    }
}
