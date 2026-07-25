using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetCategoryTrend
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

    public class GetCategoryTrendResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CategoryTrendItemResult> Categories { get; set; } = new();

        public static GetCategoryTrendResult Fail(string message) => new GetCategoryTrendResult { IsSuccess = false, Message = message };
    }

    public record GetCategoryTrendQuery(DateOnly From, DateOnly To) : IRequest<GetCategoryTrendResult>;
}
