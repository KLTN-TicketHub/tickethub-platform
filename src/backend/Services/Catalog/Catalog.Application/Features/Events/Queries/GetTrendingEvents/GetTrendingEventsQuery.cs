using Catalog.Application.Common.DTOs.Events;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetTrendingEvents
{
    public record GetTrendingEventsQuery(int Count) : IRequest<List<EventListItemDto>>;
}
