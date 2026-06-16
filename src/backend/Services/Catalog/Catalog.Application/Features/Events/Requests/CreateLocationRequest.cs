namespace Catalog.Application.Features.Events.Requests
{
    public class CreateLocationRequest
    {
        public string VenueName { get; set; }

        public string AddressLine { get; set; }

        public string Ward { get; set; }

        public string District { get; set; }

        public string ProvinceCity { get; set; }

        public string Country { get; set; }
    }
}
