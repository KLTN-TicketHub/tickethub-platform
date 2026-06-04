using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventLocation : SoftDeleteEntity
    {
        public string VenueName { get; set; }

        public string AddressLine { get; set; }

        public string Ward { get; set; }

        public string District { get; set; }

        public string ProvinceCity { get; set; }

        public string Country { get; set; }

        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public EventLocation(string venueName, string addressLine, string ward, string district, string provinceCity, string country)
        {
            VenueName = venueName;
            AddressLine = addressLine;
            Ward = ward;
            District = district;
            ProvinceCity = provinceCity;
            Country = country;
        }
    }
}
