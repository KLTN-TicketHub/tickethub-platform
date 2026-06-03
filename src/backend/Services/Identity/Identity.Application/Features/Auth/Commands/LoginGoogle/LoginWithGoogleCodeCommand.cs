using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Requests;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginGoogle
{
    public record LoginWithGoogleCodeCommand(LoginWithGoogleCodeRequest Request) : IRequest<AuthDto>;
}
