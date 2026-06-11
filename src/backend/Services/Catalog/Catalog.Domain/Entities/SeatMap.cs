using BuildingBlocks.Domain.DDD;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog.Domain.Entities
{
    public class SeatMap : SoftDeleteEntity, IAggregateRoot
    {
        public Venue? Venue { get; set; }
        public Guid VenueId { get; set; }


        public string SeatMapName { get; set; }

        public string SeatMapCode { get; private set; }

        public string? SvgFileUrl { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public byte[] RowVersion { get; private set; } = default!;

        private readonly List<Zone> _zones = new List<Zone>();
        public IReadOnlyCollection<Zone> Zones => _zones.AsReadOnly();

        public SeatMap(
            string seatMapName,
            decimal width,
            decimal height,
            string? svgFileUrl = null)
        {
            SeatMapName = seatMapName;
            Width = width;
            Height = height;
            SvgFileUrl = svgFileUrl;
        }

        public static string NormalizeSeatMapCode(string name, int maxLen = 40)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            string normalized = name.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

            string replaced = Regex.Replace(withoutDiacritics, @"[^A-Za-z0-9]+", "-");

            string trimmed = replaced.Trim('-').ToUpperInvariant();
            trimmed = Regex.Replace(trimmed, @"[^A-Z0-9\-]", string.Empty);

            if (trimmed.Length > maxLen) trimmed = trimmed.Substring(0, maxLen).Trim('-');

            return trimmed;
        }

        public void SetSeatMapCode(string seatMapCode)
        {
            SeatMapCode = seatMapCode;
        }

        public void AddZone(Zone zone)
        {
            _zones.Add(zone);
        }
    }
}
