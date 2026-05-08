using Identity.Application.Features.Auth.Request;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginAdmin.Initiate
{
    public record InitiateAdminLoginCommand(LoginRequest Request) : IRequest<Unit>;
}
