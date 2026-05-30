using BuildingBlocks.Domain.DDD;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Domain.Entities
{
    public class SeatMap : SoftDeleteEntity, IAggregateRoot
    {
        public Venue? Venue { get; set; }
        public Guid VenueId { get; set; }


        public string SeatMapName { get; set; }

        public string SeatMapCode { get; set; }

        public int Version { get; set; }

        public string? SvgFileUrl { get; set; }

        public string CanvasJsonData { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
