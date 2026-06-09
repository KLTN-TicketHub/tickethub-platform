using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class Row : SoftDeleteEntity
    {
        public Zone? Zone { get; set; }

        public Guid ZoneId { get; private set; }

        public string RowName { get; private set; }

        public CatalogStatus Status { get; set; }

        private readonly List<Seat> _seats = new List<Seat>();
        public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

        public byte[] RowVersion { get; private set; }

        public Row(string rowName)
        {
            RowName = rowName;
            Status = CatalogStatus.Active;
        }

        public void AddSeat(Seat seat)
        {
            _seats.Add(seat);
        }
    }
}