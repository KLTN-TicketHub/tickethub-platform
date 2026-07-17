using BuildingBlocks.Application.Interfaces;
using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Requests;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Queries.GetOrganizerInsights
{
    public class GetOrganizerInsightsQueryHandler : IRequestHandler<GetOrganizerInsightsQuery, OrganizerInsightsDto>
    {
        private const int TopEventsCount = 5;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetOrganizerInsightsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<OrganizerInsightsDto> Handle(GetOrganizerInsightsQuery query, CancellationToken cancellationToken)
        {
            return await GetOrganizerInsightsAsync(query.Request, cancellationToken);
        }

        private async Task<OrganizerInsightsDto> GetOrganizerInsightsAsync(GetClickTrendRequest request, CancellationToken cancellation)
        {
            Guid organizerId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Không thể xác định danh tính người dùng.");

            (DateOnly from, DateOnly to) = ResolveDateRange(request.Range);

            List<(DateOnly StatDate, EventClickType ClickType, long Total)> trendRows =
                await _unitOfWork.EventClickStatRepository.GetTrendByOrganizerAsync(organizerId, from, to, cancellation);

            List<(Guid EventId, string EventTitle, long ViewCount, long PurchaseIntentCount)> topEventRows =
                await _unitOfWork.EventClickStatRepository.GetTopEventsByOrganizerAsync(organizerId, from, to, TopEventsCount, cancellation);

            return new OrganizerInsightsDto
            {
                Trend = BuildTrendPoints(trendRows, from, to),
                TopEvents = topEventRows.Select(r => new TopEventClickStatDto
                {
                    EventId = r.EventId,
                    EventTitle = r.EventTitle,
                    ViewCount = r.ViewCount,
                    PurchaseIntentCount = r.PurchaseIntentCount
                }).ToList()
            };
        }

        private static (DateOnly From, DateOnly To) ResolveDateRange(string range)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            int days = range == "7d" ? 7 : 30;
            return (today.AddDays(-(days - 1)), today);
        }

        private static List<ClickTrendPointDto> BuildTrendPoints(List<(DateOnly StatDate, EventClickType ClickType, long Total)> rows, DateOnly from, DateOnly to)
        {
            List<ClickTrendPointDto> points = new List<ClickTrendPointDto>();

            for (DateOnly date = from; date <= to; date = date.AddDays(1))
            {
                points.Add(new ClickTrendPointDto
                {
                    Date = date,
                    ViewCount = rows.Where(r => r.StatDate == date && r.ClickType == EventClickType.ViewDetail).Sum(r => r.Total),
                    PurchaseIntentCount = rows.Where(r => r.StatDate == date && r.ClickType == EventClickType.PurchaseIntent).Sum(r => r.Total)
                });
            }

            return points;
        }
    }
}
