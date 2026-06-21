using Catalog.Application.Common.DTOs.Events;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventByIdForModerator
{
    public record GetEventByIdForModeratorQuery(Guid Id) : IRequest<EventDto>;
}
