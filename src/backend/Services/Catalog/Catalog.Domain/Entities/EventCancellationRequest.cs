using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class EventCancellationRequest : BaseEntity, IAggregateRoot
    {
        public Guid EventId { get; private set; }

        public string EventTitle { get; private set; } = default!;

        public Guid OrganizerId { get; private set; }

        public string Reason { get; private set; } = default!;

        public EventCancellationRequestStatus Status { get; private set; }

        public Guid? ReviewerUserId { get; private set; }

        public string? ReviewerName { get; private set; }

        public DateTime? ReviewedAt { get; private set; }

        public string? RejectionReason { get; private set; }

        public EventCancellationRequest(Guid eventId, string eventTitle, Guid organizerId, string reason)
        {
            EventId = eventId;
            EventTitle = eventTitle;
            OrganizerId = organizerId;
            Reason = reason;
            Status = EventCancellationRequestStatus.Pending;
        }

        public void Approve(Guid reviewerUserId, string? reviewerName)
        {
            Status = EventCancellationRequestStatus.Approved;
            ReviewerUserId = reviewerUserId;
            ReviewerName = reviewerName;
            ReviewedAt = DateTime.UtcNow;
        }

        public void Reject(Guid reviewerUserId, string? reviewerName, string reason)
        {
            Status = EventCancellationRequestStatus.Rejected;
            ReviewerUserId = reviewerUserId;
            ReviewerName = reviewerName;
            ReviewedAt = DateTime.UtcNow;
            RejectionReason = reason;
        }
    }
}
