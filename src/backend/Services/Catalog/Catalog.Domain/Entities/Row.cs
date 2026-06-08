using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class Row : SoftDeleteEntity
    {
        public Zone? Zone { get; set; }

        public Guid ZoneId { get; set; }

        public string RowName { get; set; }

        public CatalogStatus Status { get; set; }

        private readonly List<Seat> _seats = new List<Seat>();
        public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();
    }
}