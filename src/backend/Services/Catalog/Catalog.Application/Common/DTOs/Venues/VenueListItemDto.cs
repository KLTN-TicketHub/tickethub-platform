namespace Catalog.Application.Common.DTOs.Venues
{
    public class VenueListItemDto
    {
        public Guid Id { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string VenueCode { get; set; } = string.Empty;

        public string AddressLine { get; set; } = string.Empty;

        public string Ward { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string ProvinceCity { get; set; } = string.Empty;

        public int SeatMapCount { get; set; }

        public DateTime? CreateAt { get; set; }
    }
}
