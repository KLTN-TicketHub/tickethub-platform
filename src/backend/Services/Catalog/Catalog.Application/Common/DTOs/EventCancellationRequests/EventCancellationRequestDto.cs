namespace Catalog.Application.Common.DTOs.EventCancellationRequests
{
    public class EventCancellationRequestDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = default!;
        public string Reason { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? ReviewerName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
    }
}
