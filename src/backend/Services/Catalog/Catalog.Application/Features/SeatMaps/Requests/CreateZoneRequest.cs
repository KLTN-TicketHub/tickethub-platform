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
        public int Capacity { get; set; }
        public bool IsStage { get; set; }
        public bool IsReservingSeat { get; set; }
        public bool IsSalable { get; set; }
        public Guid? TicketTypeId { get; set; }
    }
}
