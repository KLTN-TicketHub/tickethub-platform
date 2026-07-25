using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.Grpc.Queries.GetEventInsight
{
    public class GetEventInsightQueryHandler : IRequestHandler<GetEventInsightQuery, GetEventInsightResult>
    {
        private const int RecentCommentsCount = 10;

        private readonly IUnitOfWork _unitOfWork;

        public GetEventInsightQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetEventInsightResult> Handle(GetEventInsightQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await GetEventInsightAsync(request.EventId, request.OrganizerId, request.From, request.To, cancellationToken);
            }
            catch (Exception ex)
            {
                return GetEventInsightResult.Fail($"Lỗi hệ thống khi lấy insight của sự kiện: {ex.Message}");
            }
        }

        private async Task<GetEventInsightResult> GetEventInsightAsync(Guid eventId, Guid organizerId, DateOnly from, DateOnly to, CancellationToken cancellationToken)
        {
            Event? eventEntity = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                filter: e => e.Id == eventId && e.OrganizerId == organizerId && !e.IsDeleted,
                cancellation: cancellationToken);

            if (eventEntity == null)
                return GetEventInsightResult.Fail($"Không tìm thấy sự kiện với ID {eventId}.");

            List<(DateOnly StatDate, EventClickType ClickType, long Total)> trendRows =
                await _unitOfWork.EventClickStatRepository.GetTrendByEventAsync(eventId, from, to, cancellationToken);

            long viewCount = trendRows.Where(r => r.ClickType == EventClickType.ViewDetail).Sum(r => r.Total);
            long purchaseIntentCount = trendRows.Where(r => r.ClickType == EventClickType.PurchaseIntent).Sum(r => r.Total);
            double conversionRate = viewCount > 0 ? Math.Round(purchaseIntentCount * 100.0 / viewCount, 1) : 0;

            List<(Guid EventId, double SoundAvg, double VisualAvg, double OrganizationAvg, double FacilityAvg, double ServiceAvg, double PerformanceAvg, double OverallAvg, int SampleSize)> ratingRows =
                await _unitOfWork.EventRatingRepository.GetRatingSummaryByEventIdsAsync(new List<Guid> { eventId }, cancellationToken);

            var rating = ratingRows.FirstOrDefault();

            List<string> recentComments = await _unitOfWork.EventRatingRepository.GetRecentCommentsByEventIdAsync(eventId, RecentCommentsCount, cancellationToken);

            return new GetEventInsightResult
            {
                IsSuccess = true,
                Message = "Thành công",
                EventTitle = eventEntity.Title,
                ViewCount = viewCount,
                PurchaseIntentCount = purchaseIntentCount,
                ConversionRate = conversionRate,
                SoundAvg = rating.SoundAvg,
                VisualAvg = rating.VisualAvg,
                OrganizationAvg = rating.OrganizationAvg,
                FacilityAvg = rating.FacilityAvg,
                ServiceAvg = rating.ServiceAvg,
                PerformanceAvg = rating.PerformanceAvg,
                OverallAvg = rating.OverallAvg,
                RatingSampleSize = rating.SampleSize,
                RecentComments = recentComments
            };
        }
    }
}
