using Catalog.Application.Features.EventCancellationRequests.Requests;
using MediatR;

namespace Catalog.Application.Features.EventCancellationRequests.Commands.RequestEventCancellation
{
    public record RequestEventCancellationCommand(Guid EventId, RequestEventCancellationRequest Request, Guid OrganizerId) : IRequest<bool>;
}
