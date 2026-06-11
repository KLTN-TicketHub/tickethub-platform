namespace Catalog.Application.Common.DTOs.SeatMaps
{
    public class ZoneDto
    {
        public Guid Id { get; set; }

        public string ZoneName { get; set; }

        public string ZoneCode { get; set; }

        public string Color { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public bool IsStage { get; set; }

        public bool IsReservingSeat { get; set; }

        public bool IsSalable { get; set; }

        public string SvgElementId { get; set; }

        public int? Capacity { get; set; }

        public decimal? BasePrice { get; set; }

        public int DisplayOrder { get; set; }

        public List<SvgElementDto>? SvgElements { get; set; }

        public List<RowDto>? Rows { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
