namespace Catalog.Application.Features.Venues.Requests
{
    public class CreateVenueRequest
    {
        public string VenueName { get; set; } = default!;

        public string AddressLine { get; set; } = default!;

        public string Ward { get; set; } = default!;

        public string District { get; set; } = default!;

        public string ProvinceCity { get; set; } = default!;

        public string Country { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public string? WebsiteUrl { get; set; }
    }
}
