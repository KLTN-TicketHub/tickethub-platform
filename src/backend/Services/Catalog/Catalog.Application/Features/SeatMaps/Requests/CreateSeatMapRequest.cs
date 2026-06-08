using Microsoft.AspNetCore.Http;

namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class CreateSeatMapRequest
    {
        public Guid VenueId { get; set; }

        public string SeatMapName { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public IFormFile SvgFile { get; set; }


    }
}
