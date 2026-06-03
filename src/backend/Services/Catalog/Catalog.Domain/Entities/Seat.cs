using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class Seat : SoftDeleteEntity, IAggregateRoot
    {
        public Zone? Zone { get; set; }
        public Guid ZoneId { get; set; }

        public string SeatCode { get; set; }

        public string SeatName { get; set; }

        public string RowLabel { get; set; }

        public string SvgElementId { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Radius { get; set; }

        public SeatLayoutStatus LayoutStatus { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
