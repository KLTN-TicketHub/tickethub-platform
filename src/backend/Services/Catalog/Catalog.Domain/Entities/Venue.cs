using BuildingBlocks.Domain.DDD;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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


        private readonly List<SeatMap> _seatMaps = new List<SeatMap>();
        public IReadOnlyCollection<SeatMap> SeatMaps => _seatMaps.AsReadOnly();

        public static string NormalizeVenueCode(string name, int maxLen = 40)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var normalized = name.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            var replaced = Regex.Replace(withoutDiacritics, @"[^A-Za-z0-9]+", "-");

            var trimmed = replaced.Trim('-').ToUpperInvariant();
            trimmed = Regex.Replace(trimmed, @"[^A-Z0-9\-]", string.Empty);

            if (trimmed.Length > maxLen) trimmed = trimmed.Substring(0, maxLen).Trim('-');

            return trimmed;
        }
    }
}
