namespace Finance.Common.Dtos.Payouts
{
    public class EventPendingPayoutDto
    {
        public Guid EventId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public Guid OrganizerId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal RecommendedRate { get; set; }
        public int OrderCount { get; set; }
    }
}
