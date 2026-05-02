using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Request;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginGoogle
{
    public record LoginGoogleCommand(GoogleLoginRequest Request) : IRequest<AuthDto>;
}
