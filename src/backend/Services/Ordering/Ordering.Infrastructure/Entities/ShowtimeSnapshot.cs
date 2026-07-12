using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    public class ShowtimeSnapshot : BaseEntity
    {
        public Guid EventSnapshotId { get; set; }
        public Guid ShowtimeId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public EventSnapshot EventSnapshot { get; set; } = default!;
        private readonly List<TicketTypeSnapshot> _ticketTypes = new();
        public IReadOnlyCollection<TicketTypeSnapshot> TicketTypes => _ticketTypes.AsReadOnly();

        public ShowtimeSnapshot(Guid eventSnapshotId, Guid showtimeId, DateTime startAt, DateTime endAt)
        {
            EventSnapshotId = eventSnapshotId;
            ShowtimeId = showtimeId;
            StartAt = startAt;
            EndAt = endAt;
        }

        public void AddTicketType(TicketTypeSnapshot ticketType)
        {
            _ticketTypes.Add(ticketType);
        }
    }
}
