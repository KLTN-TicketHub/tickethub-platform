using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetOrganizerPortfolio
{
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

    public class GetOrganizerPortfolioResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrganizerPortfolioCategoryResult> CategoryDistribution { get; set; } = new();
        public List<OrganizerPortfolioEventResult> Events { get; set; } = new();

        public static GetOrganizerPortfolioResult Fail(string message) => new GetOrganizerPortfolioResult { IsSuccess = false, Message = message };
    }

    public record GetOrganizerPortfolioQuery(Guid OrganizerId, DateOnly From, DateOnly To) : IRequest<GetOrganizerPortfolioResult>;
}
