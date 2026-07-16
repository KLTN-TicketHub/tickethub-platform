using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventCheckIn : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }

        public Guid UserId { get; set; }

        public Guid IssuedTicketId { get; set; }

        public DateTime CheckedInAt { get; set; }

        public EventCheckIn(Guid eventId, Guid userId, Guid issuedTicketId, DateTime checkedInAt)
        {
            EventId = eventId;
            UserId = userId;
            IssuedTicketId = issuedTicketId;
            CheckedInAt = checkedInAt;
        }
    }
}
