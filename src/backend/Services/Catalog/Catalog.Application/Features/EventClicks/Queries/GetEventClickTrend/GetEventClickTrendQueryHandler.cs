using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Requests;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Queries.GetEventClickTrend
{
    public class GetEventClickTrendQueryHandler : IRequestHandler<GetEventClickTrendQuery, List<ClickTrendPointDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetEventClickTrendQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<ClickTrendPointDto>> Handle(GetEventClickTrendQuery query, CancellationToken cancellationToken)
        {
            return await GetEventClickTrendAsync(query.EventId, query.Request, cancellationToken);
        }

        private async Task<List<ClickTrendPointDto>> GetEventClickTrendAsync(Guid eventId, GetClickTrendRequest request, CancellationToken cancellation)
        {
            _ = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                filter: e => e.Id == eventId && e.OrganizerId == _currentUserService.UserId && !e.IsDeleted,
                cancellation: cancellation)
                ?? throw new NotFoundException($"Không tìm thấy sự kiện với ID {eventId}.");

            (DateOnly from, DateOnly to) = ResolveDateRange(request.Range);

            List<(DateOnly StatDate, EventClickType ClickType, long Total)> rows =
                await _unitOfWork.EventClickStatRepository.GetTrendByEventAsync(eventId, from, to, cancellation);

            return BuildTrendPoints(rows, from, to);
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
