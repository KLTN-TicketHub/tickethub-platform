using Identity.Application.Common.DTOs.Admin;
using Identity.Application.Features.Admin.Moderators.Requests;
using MediatR;

namespace Identity.Application.Features.Admin.Moderators.Commands.RegisterModerator
{
    public record RegisterModeratorCommand(RegisterModeratorRequest Request) : IRequest<ModeratorDto>;
}
