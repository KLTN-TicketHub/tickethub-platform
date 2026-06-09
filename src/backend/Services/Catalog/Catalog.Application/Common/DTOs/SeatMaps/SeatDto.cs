using Catalog.Domain.Enums;

namespace Catalog.Application.Common.DTOs.SeatMaps
{
    public class SeatDto
    {
        public Guid Id { get; set; }

        public string SeatName { get; set; }

        public string SeatCode { get; set; } 

        public string SvgElementId { get; set; }

        public decimal X { get; set; }

        public decimal Y { get; set; }

        public decimal Radius { get; set; }

        public SeatLayoutStatus LayoutStatus { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
