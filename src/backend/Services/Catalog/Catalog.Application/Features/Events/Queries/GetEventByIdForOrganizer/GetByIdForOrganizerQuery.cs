using Catalog.Application.Common.DTOs.Events;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventByIdForOrganizer
{
    public record GetByIdForOrganizerQuery(Guid Id) : IRequest<EventDto>;
}
