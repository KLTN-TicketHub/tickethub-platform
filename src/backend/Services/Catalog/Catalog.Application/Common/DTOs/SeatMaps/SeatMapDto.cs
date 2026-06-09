namespace Catalog.Application.Common.DTOs.SeatMaps
{
    public class SeatMapDto
    {
        public Guid Id { get; set; }

        public Guid VenueId { get; set; }

        public string SeatMapName { get; set; }

        public string SeatMapCode { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public string? SvgFileUrl { get; set; }

        public List<ZoneDto>? Zones { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
