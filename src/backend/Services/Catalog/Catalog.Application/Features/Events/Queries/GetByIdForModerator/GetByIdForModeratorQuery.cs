using Catalog.Application.Common.DTOs.Events;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetByIdForModerator
{
    public record GetByIdForModeratorQuery(Guid Id) : IRequest<EventDto>;
}
