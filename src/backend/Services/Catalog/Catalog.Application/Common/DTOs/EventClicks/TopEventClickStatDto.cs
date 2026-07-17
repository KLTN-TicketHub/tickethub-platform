namespace Catalog.Application.Common.DTOs.EventClicks
{
    public class TopEventClickStatDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public long ViewCount { get; set; }
        public long PurchaseIntentCount { get; set; }
    }
}
