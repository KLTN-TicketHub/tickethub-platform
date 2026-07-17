namespace Catalog.Application.Common.DTOs.EventClicks
{
    public class ClickTrendPointDto
    {
        public DateOnly Date { get; set; }
        public long ViewCount { get; set; }
        public long PurchaseIntentCount { get; set; }
    }
}
