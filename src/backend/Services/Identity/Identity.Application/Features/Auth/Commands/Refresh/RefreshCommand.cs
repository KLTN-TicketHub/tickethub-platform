using Identity.Application.Common.DTOs.Auth;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Refresh
{
    public record RefreshCommand(string RefreshToken) : IRequest<AuthDto>;
}
