namespace Finance.Common.Dtos.Reports
{
    public class AdminFinanceSummaryDto
    {
        public decimal GrossRevenue { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetPaidToOrganizers { get; set; }
        public int EventsSettledCount { get; set; }
        public int PendingPayoutRequestsCount { get; set; }
        public decimal CurrentTotalWalletBalance { get; set; }
    }
}
