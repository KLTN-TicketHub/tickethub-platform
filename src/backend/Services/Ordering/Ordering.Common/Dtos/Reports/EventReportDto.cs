namespace Ordering.Common.Dtos.Reports
{
    public class EventReportDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public string EventImage { get; set; } = default!;
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
        public int TotalCapacity { get; set; }
        public double FillRate { get; set; }
        public List<ShowtimeReportDto> Showtimes { get; set; } = new();
    }

    public class ShowtimeReportDto
    {
        public Guid ShowtimeId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal Revenue { get; set; }
        public int TicketsSold { get; set; }
        public int Capacity { get; set; }
        public double FillRate { get; set; }
    }
}
