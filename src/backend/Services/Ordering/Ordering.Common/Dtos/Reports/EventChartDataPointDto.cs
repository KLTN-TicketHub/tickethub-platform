namespace Ordering.Common.Dtos.Reports
{
    public class EventChartDataPointDto
    {
        public decimal TotalAmount { get; set; }
        public int TicketSold { get; set; }
        public string Legend { get; set; } = default!;
    }
}
