namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class CreateSeatRequest
    {
        public string SeatName { get; set; }

        public string SvgElementId { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Radius { get; set; }
    }
}
