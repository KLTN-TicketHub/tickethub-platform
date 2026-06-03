using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Requests;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.LoginStaff
{
    public record LoginStaffCommand(LoginRequest Request) : IRequest<AuthDto>;
}
