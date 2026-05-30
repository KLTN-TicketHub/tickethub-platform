using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class EventApproval : SoftDeleteEntity, IAggregateRoot
    {
        public Event? Event { get; set; }
        public Guid EventId { get; set; }

        //Trạng thái duyệt
        public string ApprovalStatus { get; set; }

        public string? Reason { get; set; }

        public string? ReviewerUserId { get; set; }

        public string? ReviewerName { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
