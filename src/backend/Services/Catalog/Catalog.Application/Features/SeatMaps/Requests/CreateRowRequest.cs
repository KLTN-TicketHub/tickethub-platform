namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class CreateRowRequest
    {
        public string RowLabel { get; set; }

        public List<CreateSeatRequest> SeatRequests { get; set; }
    }
}
