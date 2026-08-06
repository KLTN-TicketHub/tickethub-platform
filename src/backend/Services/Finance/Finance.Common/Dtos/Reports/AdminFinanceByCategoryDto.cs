namespace Finance.Common.Dtos.Reports
{
    public class AdminFinanceByCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public decimal GrossRevenue { get; set; }
        public decimal PlatformFee { get; set; }
        public int EventCount { get; set; }
    }
}
