namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class CreateSeatMapRequest
    {
        public Guid VenueId { get; set; }

        public string SeatMapName { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public string? SvgFileUrl { get; set; }

        public List<CreateZoneRequest> Zones { get; set; }
    }
}
