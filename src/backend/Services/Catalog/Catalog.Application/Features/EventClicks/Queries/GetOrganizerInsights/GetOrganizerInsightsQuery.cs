using Catalog.Application.Common.DTOs.EventClicks;
using Catalog.Application.Features.EventClicks.Requests;
using MediatR;

namespace Catalog.Application.Features.EventClicks.Queries.GetOrganizerInsights
{
    public record GetOrganizerInsightsQuery(GetClickTrendRequest Request) : IRequest<OrganizerInsightsDto>;
}
