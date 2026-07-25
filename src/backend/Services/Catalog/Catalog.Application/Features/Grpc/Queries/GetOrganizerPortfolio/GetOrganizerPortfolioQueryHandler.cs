using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetOrganizerPortfolio
{
    public class GetOrganizerPortfolioQueryHandler : IRequestHandler<GetOrganizerPortfolioQuery, GetOrganizerPortfolioResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrganizerPortfolioQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetOrganizerPortfolioResult> Handle(GetOrganizerPortfolioQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await GetOrganizerPortfolioAsync(request.OrganizerId, request.From, request.To, cancellationToken);
            }
            catch (Exception ex)
            {
                return GetOrganizerPortfolioResult.Fail($"Lỗi hệ thống khi lấy portfolio của organizer: {ex.Message}");
            }
        }

        private async Task<GetOrganizerPortfolioResult> GetOrganizerPortfolioAsync(Guid organizerId, DateOnly from, DateOnly to, CancellationToken cancellationToken)
        {
            var events = await _unitOfWork.EventRepository.GetAllAsync(
                filter: e => e.OrganizerId == organizerId && !e.IsDeleted && e.Status == EventStatus.Published,
                selector: e => new { e.Id, e.Title, e.CategoryId, CategoryName = e.Category!.CategoryName },
                cancellation: cancellationToken);

            var eventList = events.ToList();

            List<OrganizerPortfolioCategoryResult> categoryDistribution = eventList
                .GroupBy(e => new { e.CategoryId, e.CategoryName })
                .Select(g => new OrganizerPortfolioCategoryResult
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    EventCount = g.Count()
                })
                .ToList();

            List<Guid> eventIds = eventList.Select(e => e.Id).ToList();

            List<(Guid EventId, string EventTitle, long ViewCount, long PurchaseIntentCount)> clickRows =
                await _unitOfWork.EventClickStatRepository.GetTopEventsByOrganizerAsync(organizerId, from, to, int.MaxValue, cancellationToken);

            List<(Guid EventId, double SoundAvg, double VisualAvg, double OrganizationAvg, double FacilityAvg, double ServiceAvg, double PerformanceAvg, double OverallAvg, int SampleSize)> ratingRows =
                await _unitOfWork.EventRatingRepository.GetRatingSummaryByEventIdsAsync(eventIds, cancellationToken);

            Dictionary<Guid, (long ViewCount, long PurchaseIntentCount)> clickByEvent =
                clickRows.ToDictionary(r => r.EventId, r => (r.ViewCount, r.PurchaseIntentCount));

            Dictionary<Guid, (double SoundAvg, double VisualAvg, double OrganizationAvg, double FacilityAvg, double ServiceAvg, double PerformanceAvg, double OverallAvg, int SampleSize)> ratingByEvent =
                ratingRows.ToDictionary(r => r.EventId, r => (r.SoundAvg, r.VisualAvg, r.OrganizationAvg, r.FacilityAvg, r.ServiceAvg, r.PerformanceAvg, r.OverallAvg, r.SampleSize));

            List<OrganizerPortfolioEventResult> eventResults = eventList.Select(e =>
            {
                (long ViewCount, long PurchaseIntentCount) click = clickByEvent.TryGetValue(e.Id, out var c) ? c : (0, 0);
                var rating = ratingByEvent.TryGetValue(e.Id, out var r) ? r : (0, 0, 0, 0, 0, 0, 0, 0);

                double conversionRate = click.ViewCount > 0 ? Math.Round(click.PurchaseIntentCount * 100.0 / click.ViewCount, 1) : 0;

                return new OrganizerPortfolioEventResult
                {
                    EventId = e.Id,
                    EventTitle = e.Title,
                    ViewCount = click.ViewCount,
                    PurchaseIntentCount = click.PurchaseIntentCount,
                    ConversionRate = conversionRate,
                    OverallRatingAvg = rating.OverallAvg,
                    RatingSampleSize = rating.SampleSize,
                    LowestRatingDimension = GetLowestRatingDimension(rating.SoundAvg, rating.VisualAvg, rating.OrganizationAvg, rating.FacilityAvg, rating.ServiceAvg, rating.PerformanceAvg)
                };
            }).ToList();

            return new GetOrganizerPortfolioResult
            {
                IsSuccess = true,
                Message = "Thành công",
                CategoryDistribution = categoryDistribution,
                Events = eventResults
            };
        }

        private static string GetLowestRatingDimension(double sound, double visual, double organization, double facility, double service, double performance)
        {
            (string Name, double Value)[] dimensions = new (string Name, double Value)[]
            {
                ("Sound", sound),
                ("Visual", visual),
                ("Organization", organization),
                ("Facility", facility),
                ("Service", service),
                ("Performance", performance)
            };

            return dimensions.OrderBy(d => d.Value).First().Name;
        }
    }
}
