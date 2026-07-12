using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    /// <summary>
    /// Local snapshot of event data replicated from Catalog service via EventPublishedEvent.
    /// Used by Ordering to avoid cross-service gRPC calls during checkout and reporting.
    /// </summary>
    public class EventSnapshot : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }

        public string EventTitle { get; set; } = default!;

        public string EventImage { get; set; } = default!;

        public Guid OrganizerId { get; set; }

        public string OrganizerName { get; set; } = default!;

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        public DateTime SnapshotCreatedAt { get; set; }

        private readonly List<ShowtimeSnapshot> _showtimes = new();
        public IReadOnlyCollection<ShowtimeSnapshot> Showtimes => _showtimes.AsReadOnly();

        public EventSnapshot(
            Guid eventId,
            string eventTitle,
            string eventImage,
            Guid organizerId,
            string organizerName,
            DateTime saleOpenAt,
            DateTime saleCloseAt)
        {
            EventId = eventId;
            EventTitle = eventTitle;
            EventImage = eventImage;
            OrganizerId = organizerId;
            OrganizerName = organizerName;
            SaleOpenAt = saleOpenAt;
            SaleCloseAt = saleCloseAt;
            SnapshotCreatedAt = DateTime.UtcNow;
        }

        public void AddShowtime(ShowtimeSnapshot showtime)
        {
            _showtimes.Add(showtime);
        }
    }
}
