using Catalog.Application.Features.Events.Requests;
using MediatR;

namespace Catalog.Application.Features.Events.Commands.ReviewEvent
{
    public record ReviewEventCommand(Guid EventId, ReviewEventRequest Request, Guid ReviewerUserId, string? ReviewerName) : IRequest<bool>;
}
