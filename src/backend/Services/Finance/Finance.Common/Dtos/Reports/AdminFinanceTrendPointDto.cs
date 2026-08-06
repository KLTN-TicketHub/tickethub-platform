namespace Finance.Common.Dtos.Reports
{
    public class AdminFinanceTrendPointDto
    {
        public DateTime Date { get; set; }
        public decimal GrossRevenue { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetAmount { get; set; }
    }
}
