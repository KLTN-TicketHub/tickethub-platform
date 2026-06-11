namespace Catalog.Application.Common.DTOs.SeatMaps
{
    public class SeatMapListItemDto
    {
        public Guid Id { get; set; }

        public Guid? VenueId { get; set; }

        public string SeatMapName { get; set; }

        public string SeatMapCode { get; set; }
    }
}
