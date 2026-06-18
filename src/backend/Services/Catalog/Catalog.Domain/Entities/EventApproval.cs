using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventApproval : SoftDeleteEntity
    {
        public Event? Event { get; set; }
        public Guid EventId { get; set; }

        public string? Reason { get; set; }

        public Guid ReviewerUserId { get; set; }

        public string? ReviewerName { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
