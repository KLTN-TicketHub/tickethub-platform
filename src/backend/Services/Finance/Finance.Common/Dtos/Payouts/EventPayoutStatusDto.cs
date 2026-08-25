namespace Finance.Common.Dtos.Payouts
{
    public class EventPayoutStatusDto
    {
        public bool HasPendingRequest { get; set; }

        public bool HasProposedPayout { get; set; }

        public bool HasAcceptedPayout { get; set; }

        public Guid? ProposedPayoutId { get; set; }
    }
}
