using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Request;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginModerator
{
    public record LoginModeratorCommand(LoginRequest Request) : IRequest<AuthDto>;
}
