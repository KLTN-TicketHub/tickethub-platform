using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventCancellation : SoftDeleteEntity
    {
        public Event? Event { get; set; }
        public Guid EventId { get; set; }

        public Guid CancelledByUserId { get; set; }

        public string? CancelledByName { get; set; }

        public string? Reason { get; set; }

        public DateTime CancelledAt { get; set; }
    }
}
