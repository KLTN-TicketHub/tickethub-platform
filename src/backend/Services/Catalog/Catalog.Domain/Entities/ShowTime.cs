using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class ShowTime : SoftDeleteEntity
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; private set; }

        private readonly List<TicketType> _ticketTypes = new List<TicketType>();
        public IReadOnlyCollection<TicketType> TicketTypes => _ticketTypes.AsReadOnly();

        public ShowTime(DateTime startAt, DateTime endAt)
        {
            StartAt = startAt;
            EndAt = endAt;
            Status = CatalogStatus.Active;
        }

        public void AddTicketType(TicketType ticketType)
        {
            _ticketTypes.Add(ticketType);
        }
    }
}
