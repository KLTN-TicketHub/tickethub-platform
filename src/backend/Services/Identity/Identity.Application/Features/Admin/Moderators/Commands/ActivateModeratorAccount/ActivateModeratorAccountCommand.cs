using Identity.Application.Features.Admin.Moderators.Requests;
using MediatR;

namespace Identity.Application.Features.Admin.Moderators.Commands.ActivateModeratorAccount
{
    public record ActivateModeratorAccountCommand(ActivateModeratorAccountRequest Request) : IRequest<Unit>;
}
