using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Request;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginAdmin
{
    public record LoginAdminCommand(LoginRequest Request) : IRequest<AuthDto>;
}
