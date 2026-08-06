namespace Payment.Common.Dtos.Reports
{
    public class AdminPaymentGatewayStatsDto
    {
        public string Gateway { get; set; } = default!;
        public int TotalTransactions { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
        public decimal SuccessAmount { get; set; }
        public double SuccessRate { get; set; }
    }
}
