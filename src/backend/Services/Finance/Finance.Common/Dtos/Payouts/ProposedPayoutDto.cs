namespace Finance.Common.Dtos.Payouts
{
    public class ProposedPayoutDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public decimal GrossAmount { get; set; }
        public decimal AppliedRate { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime ReviewedAt { get; set; }
    }
}
