namespace Finance.Common.Dtos.Payouts
{
    public class PayoutRequestDto
    {
        public Guid PayoutRequestId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = default!;
        public decimal GrossAmount { get; set; }
        public decimal RecommendedRate { get; set; }
        public int OrderCount { get; set; }
        public DateTime RequestedAt { get; set; }
        public bool IsResubmitted { get; set; }
        public string? LastRejectionReason { get; set; }
        public DateTime? LastRejectedAt { get; set; }
    }
}
