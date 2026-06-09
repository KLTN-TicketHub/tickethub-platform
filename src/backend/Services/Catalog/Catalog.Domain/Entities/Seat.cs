using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog.Domain.Entities
{
    public class Seat : SoftDeleteEntity, IAggregateRoot
    {
        public Row? Row { get; set; }
        public Guid RowId { get; set; }

        public string SeatCode { get; private set; }

        public string SeatName { get; set; }

        public string SvgElementId { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Radius { get; set; }

        public SeatLayoutStatus LayoutStatus { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        public Seat(
            string seatName,
            string svgElementId,
            decimal x,
            decimal y,
            decimal radius)
        {
            SeatName = seatName;
            SvgElementId = svgElementId;
            X = x;
            Y = y;
            Radius = radius;
            LayoutStatus = SeatLayoutStatus.Available;
        }

        public static string NormalizeSeatCode(string name, int maxLen = 40)
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

        public void SetSeatCode(string seatCode)
        {
            SeatCode = seatCode;
        }
    }
}
