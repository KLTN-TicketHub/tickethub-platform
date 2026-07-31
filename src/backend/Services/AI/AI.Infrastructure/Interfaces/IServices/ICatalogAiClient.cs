namespace AI.Infrastructure.Interfaces.IServices
{
    public class CategoryTrendItemResult
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public long ViewCount { get; set; }
        public long PurchaseIntentCount { get; set; }
        public int ActiveEventCount { get; set; }
        public double ViewGrowthPercent { get; set; }
        public double AvgOverallRating { get; set; }
        public int RatingSampleSize { get; set; }
    }

    public class CategoryTrendResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CategoryTrendItemResult> Categories { get; set; } = new();
    }

    public class OrganizerPortfolioCategoryResult
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int EventCount { get; set; }
    }

    public class OrganizerPortfolioEventResult
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public long ViewCount { get; set; }
        public long PurchaseIntentCount { get; set; }
        public double ConversionRate { get; set; }
        public double OverallRatingAvg { get; set; }
        public int RatingSampleSize { get; set; }
        public string LowestRatingDimension { get; set; } = string.Empty;
    }

    public class OrganizerPortfolioResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizerPortfolioCategoryResult> CategoryDistribution { get; set; } = new();
        public List<OrganizerPortfolioEventResult> Events { get; set; } = new();
    }

    public class EventInsightResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public long ViewCount { get; set; }
        public long PurchaseIntentCount { get; set; }
        public double ConversionRate { get; set; }
        public double SoundAvg { get; set; }
        public double VisualAvg { get; set; }
        public double OrganizationAvg { get; set; }
        public double FacilityAvg { get; set; }
        public double ServiceAvg { get; set; }
        public double PerformanceAvg { get; set; }
        public double OverallAvg { get; set; }
        public int RatingSampleSize { get; set; }
        public List<string> RecentComments { get; set; } = new();
    }

    public class EventSummaryResult
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public decimal MinPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string ProvinceCity { get; set; } = string.Empty;
    }

    public class SearchEventsResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<EventSummaryResult> Events { get; set; } = new();
    }

    public class EventTicketTypeResult
    {
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int PublishedQuota { get; set; }
    }

    public class EventShowtimeResult
    {
        public Guid ShowtimeId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<EventTicketTypeResult> TicketTypes { get; set; } = new();
    }

    public class EventDetailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ProvinceCity { get; set; } = string.Empty;
        public string VenueName { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public List<EventShowtimeResult> Showtimes { get; set; } = new();
    }

    public interface ICatalogAiClient
    {
        Task<CategoryTrendResult> GetCategoryTrendAsync(DateOnly from, DateOnly to);

        Task<OrganizerPortfolioResult> GetOrganizerPortfolioAsync(Guid organizerId, DateOnly from, DateOnly to);

        Task<EventInsightResult> GetEventInsightAsync(Guid eventId, Guid organizerId, DateOnly from, DateOnly to);

        Task<SearchEventsResult> SearchEventsAsync(string search, Guid? categoryId, string? provinceCity, int pageSize);

        Task<EventDetailResult> GetEventDetailAsync(Guid eventId);
    }
}
