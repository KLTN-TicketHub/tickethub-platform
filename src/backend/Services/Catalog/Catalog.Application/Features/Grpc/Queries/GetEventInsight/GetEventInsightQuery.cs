using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetEventInsight
{
    public class GetEventInsightResult
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

        public static GetEventInsightResult Fail(string message) => new GetEventInsightResult { IsSuccess = false, Message = message };
    }

    public record GetEventInsightQuery(Guid EventId, Guid OrganizerId, DateOnly From, DateOnly To) : IRequest<GetEventInsightResult>;
}
