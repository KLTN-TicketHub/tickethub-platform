using Catalog.Application.Features.EventCancellationRequests.Requests;
using MediatR;

namespace Catalog.Application.Features.EventCancellationRequests.Commands.ReviewEventCancellationRequest
{
    public record ReviewEventCancellationRequestCommand(Guid RequestId, ReviewEventCancellationRequestRequest Request, Guid ReviewerUserId, string? ReviewerName) : IRequest<bool>;
}
