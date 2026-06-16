using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using MediatR;

namespace Catalog.Application.Features.Events.Commands.CreateEvent
{
    public record CreateEventCommand(Guid OrganizerId, CreateEventRequest Request) : IRequest<EventDto>;
}
