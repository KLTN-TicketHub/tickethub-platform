namespace Finance.Common.Dtos.Payouts
{
    public class EventPayoutResultDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid OrganizerId { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal AppliedRate { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime ReviewedAt { get; set; }
    }
}
