using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class UserEventClick : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public Guid UserId { get; set; }

        public EventClickType ClickType { get; set; }

        public DateTime ClickedAt { get; set; }

        public UserEventClick(Guid eventId, Guid userId, EventClickType clickType, DateTime clickedAt)
        {
            EventId = eventId;
            UserId = userId;
            ClickType = clickType;
            ClickedAt = clickedAt;
        }
    }
}
