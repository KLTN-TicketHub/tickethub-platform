using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Catalog.Domain.Entities
{
    public class Zone : SoftDeleteEntity, IAggregateRoot
    {
        public SeatMap? SeatMap { get; set; }
        public Guid SeatMapId { get; set; }

        public string ZoneName { get; set; }

        public string ZoneCode { get; private set; }

        public string Color { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public bool IsStage { get; set; }

        //Phân khu có ghế hoặc không
        public bool IsReservingSeat { get; set; }

        public bool IsSalable { get; set; }

        public string SvgElementId { get; set; }

        public string? ElementJson { get; private set; }

        public int? Capacity { get; set; }

        public decimal? BasePrice { get; set; }

        public int DisplayOrder { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        private readonly List<Row> _rows = new List<Row>();
        public IReadOnlyCollection<Row> Rows => _rows.AsReadOnly();

        public Zone(
            string zoneName,
            string color,
            decimal x,
            decimal y,
            decimal width,
            decimal height,
            bool isStage,
            bool isReservingSeat,
            bool isSalable,
            string svgElementId,
            int? capacity = null,
            decimal? basePrice = null,
            int displayOrder = 0)
        {
            ZoneName = zoneName;
            Color = color;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            IsStage = isStage;
            IsReservingSeat = isReservingSeat;
            IsSalable = isSalable;
            SvgElementId = svgElementId;
            Capacity = capacity;
            BasePrice = basePrice;
            DisplayOrder = displayOrder;
            Status = CatalogStatus.Active;
        }

        public static string NormalizeZoneCode(string name, int maxLen = 40)
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
        public void SetZoneCode(string zoneCode)
        {
            ZoneCode = zoneCode;
        }

        public void AddRow(Row row)
        {
            _rows.Add(row);
        }

        public void SetElementJson(string elementJson)
        {
            ElementJson = elementJson;
        }
    }
}
