namespace Catalog.Application.Common.DTOs.EventClicks
{
    public class OrganizerInsightsDto
    {
        public long TotalViews { get; set; }
        public List<ClickTrendPointDto> Trend { get; set; } = new();
        public List<TopEventClickStatDto> TopEvents { get; set; } = new();
    }
}
