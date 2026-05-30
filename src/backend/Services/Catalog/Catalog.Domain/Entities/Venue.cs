using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class Venue : SoftDeleteEntity, IAggregateRoot
    {
        public string VenueName { get; set; }

        public string VenueCode { get; set; }

        //Địa chỉ chi tiết
        public string AddressLine { get; set; }

        //Phường/Xã
        public string Ward { get; set; }

        //Quận/Huyện
        public string District { get; set; }

        //Tỉnh/Thành phố
        public string ProvinceCity { get; set; }

        //Quốc gia
        public string Country { get; set; }

        public string Slug { get; set; }

        //Kinh độ
        public decimal Longitude { get; set; }

        //Vĩ độ
        public decimal Latitude { get; set; }

        //Tổng sức chứa
        public int TotalCapacity { get; set; }

        public string PhoneNumber { get; set; }

        public string? WebsiteUrl { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
