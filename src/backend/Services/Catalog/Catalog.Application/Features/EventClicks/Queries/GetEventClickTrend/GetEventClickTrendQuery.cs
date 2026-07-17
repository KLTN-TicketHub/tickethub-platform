using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Requests;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Queries.GetEventClickTrend
{
    public record GetEventClickTrendQuery(Guid EventId, GetClickTrendRequest Request) : IRequest<List<ClickTrendPointDto>>;
}
