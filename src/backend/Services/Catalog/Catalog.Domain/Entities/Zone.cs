using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class Zone : SoftDeleteEntity, IAggregateRoot
    {
        public SeatMap? SeatMap { get; set; }
        public Guid SeatMapId { get; set; }

        public string ZoneName { get; set; }

        public string ZoneCode { get; set; }

        public string ZoneType { get; set; }

        public string Color { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public string SvgElementId { get; set; }

        public int Capacity { get; set; }

        public decimal BasePrice { get; set; }

        public int DisplayOrder { get; set; }

        public string Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
