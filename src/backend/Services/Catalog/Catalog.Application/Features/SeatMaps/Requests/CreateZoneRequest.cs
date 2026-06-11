namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class CreateZoneRequest
    {
        public string ZoneName { get; set; }

        public string Color { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public int? Capacity { get; set; }

        public bool IsStage { get; set; }

        //Phân khu có ghế hoặc không
        public bool IsReservingSeat { get; set; }

        //Phân khu có bán được vé hay không
        public bool IsSalable { get; set; }

        public string SvgElementId { get; set; }

        public decimal? BasePrice { get; set; }

        public int? DisplayOrder { get; set; }

        public List<CreateSvgElementRequest>? SvgElements { get; set; }

        public List<CreateRowRequest>? Rows { get; set; }
    }
}
