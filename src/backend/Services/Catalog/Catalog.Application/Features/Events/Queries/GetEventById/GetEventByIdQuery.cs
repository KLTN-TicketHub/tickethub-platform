using Catalog.Application.Common.DTOs.Events;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventById
{
    public record GetEventByIdQuery(Guid Id) : IRequest<EventDto>;
}
