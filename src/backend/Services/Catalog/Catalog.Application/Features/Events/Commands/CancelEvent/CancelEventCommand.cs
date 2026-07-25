using MediatR;

namespace Catalog.Application.Features.Events.Commands.CancelEvent
{
    public record CancelEventCommand(Guid EventId, Guid ModeratorUserId, string? ModeratorName) : IRequest<bool>;
}
