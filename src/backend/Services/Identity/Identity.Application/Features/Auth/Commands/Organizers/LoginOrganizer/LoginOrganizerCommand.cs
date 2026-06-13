using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Requests;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Organizers.LoginOrganizer
{
    public record LoginOrganizerCommand(LoginRequest Request) : IRequest<AuthDto>;
}
