using AI.Infrastructure.Interfaces.IServices;
using Catalog.API.Protos;
using Grpc.Core;

namespace AI.Infrastructure.Services
{
    public class CatalogAiClient : ICatalogAiClient
    {
        private readonly CatalogGrpc.CatalogGrpcClient _client;

        public CatalogAiClient(CatalogGrpc.CatalogGrpcClient client)
        {
            _client = client;
        }

        public async Task<CategoryTrendResult> GetCategoryTrendAsync(DateOnly from, DateOnly to)
        {
            try
            {
                GetCategoryTrendRequest request = new GetCategoryTrendRequest
                {
                    From = from.ToString("yyyy-MM-dd"),
                    To = to.ToString("yyyy-MM-dd")
                };

                GetCategoryTrendResponse response = await _client.GetCategoryTrendAsync(request);

                if (!response.IsSuccess)
                    return new CategoryTrendResult { IsSuccess = false, Message = response.Message };

                CategoryTrendResult result = new CategoryTrendResult { IsSuccess = true, Message = response.Message };

                foreach (var c in response.Categories)
                {
                    result.Categories.Add(new CategoryTrendItemResult
                    {
                        CategoryId = Guid.Parse(c.CategoryId),
                        CategoryName = c.CategoryName,
                        ViewCount = c.ViewCount,
                        PurchaseIntentCount = c.PurchaseIntentCount,
                        ActiveEventCount = c.ActiveEventCount,
                        ViewGrowthPercent = c.ViewGrowthPercent,
                        AvgOverallRating = c.AvgOverallRating,
                        RatingSampleSize = c.RatingSampleSize
                    });
                }

                return result;
            }
            catch (RpcException ex)
            {
                return new CategoryTrendResult { IsSuccess = false, Message = $"Lỗi kết nối gRPC tới Catalog: {ex.Status.Detail}" };
            }
        }

        public async Task<OrganizerPortfolioResult> GetOrganizerPortfolioAsync(Guid organizerId, DateOnly from, DateOnly to)
        {
            try
            {
                GetOrganizerPortfolioRequest request = new GetOrganizerPortfolioRequest
                {
                    OrganizerId = organizerId.ToString(),
                    From = from.ToString("yyyy-MM-dd"),
                    To = to.ToString("yyyy-MM-dd")
                };

                GetOrganizerPortfolioResponse response = await _client.GetOrganizerPortfolioAsync(request);

                if (!response.IsSuccess)
                    return new OrganizerPortfolioResult { IsSuccess = false, Message = response.Message };

                OrganizerPortfolioResult result = new OrganizerPortfolioResult { IsSuccess = true, Message = response.Message };

                foreach (var c in response.CategoryDistribution)
                {
                    result.CategoryDistribution.Add(new OrganizerPortfolioCategoryResult
                    {
                        CategoryId = Guid.Parse(c.CategoryId),
                        CategoryName = c.CategoryName,
                        EventCount = c.EventCount
                    });
                }

                foreach (var e in response.Events)
                {
                    result.Events.Add(new OrganizerPortfolioEventResult
                    {
                        EventId = Guid.Parse(e.EventId),
                        EventTitle = e.EventTitle,
                        ViewCount = e.ViewCount,
                        PurchaseIntentCount = e.PurchaseIntentCount,
                        ConversionRate = e.ConversionRate,
                        OverallRatingAvg = e.OverallRatingAvg,
                        RatingSampleSize = e.RatingSampleSize,
                        LowestRatingDimension = e.LowestRatingDimension
                    });
                }

                return result;
            }
            catch (RpcException ex)
            {
                return new OrganizerPortfolioResult { IsSuccess = false, Message = $"Lỗi kết nối gRPC tới Catalog: {ex.Status.Detail}" };
            }
        }

        public async Task<EventInsightResult> GetEventInsightAsync(Guid eventId, Guid organizerId, DateOnly from, DateOnly to)
        {
            try
            {
                GetEventInsightRequest request = new GetEventInsightRequest
                {
                    EventId = eventId.ToString(),
                    OrganizerId = organizerId.ToString(),
                    From = from.ToString("yyyy-MM-dd"),
                    To = to.ToString("yyyy-MM-dd")
                };

                GetEventInsightResponse response = await _client.GetEventInsightAsync(request);

                if (!response.IsSuccess)
                    return new EventInsightResult { IsSuccess = false, Message = response.Message };

                return new EventInsightResult
                {
                    IsSuccess = true,
                    Message = response.Message,
                    EventTitle = response.EventTitle,
                    ViewCount = response.ViewCount,
                    PurchaseIntentCount = response.PurchaseIntentCount,
                    ConversionRate = response.ConversionRate,
                    SoundAvg = response.SoundAvg,
                    VisualAvg = response.VisualAvg,
                    OrganizationAvg = response.OrganizationAvg,
                    FacilityAvg = response.FacilityAvg,
                    ServiceAvg = response.ServiceAvg,
                    PerformanceAvg = response.PerformanceAvg,
                    OverallAvg = response.OverallAvg,
                    RatingSampleSize = response.RatingSampleSize,
                    RecentComments = response.RecentComments.ToList()
                };
            }
            catch (RpcException ex)
            {
                return new EventInsightResult { IsSuccess = false, Message = $"Lỗi kết nối gRPC tới Catalog: {ex.Status.Detail}" };
            }
        }
    }
}
