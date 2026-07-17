using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class EventClickStat : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public DateOnly StatDate { get; set; }

        public EventClickType ClickType { get; set; }

        public long ClickCount { get; set; }

        public EventClickStat(Guid eventId, DateOnly statDate, EventClickType clickType, long clickCount)
        {
            EventId = eventId;
            StatDate = statDate;
            ClickType = clickType;
            ClickCount = clickCount;
        }
    }
}
