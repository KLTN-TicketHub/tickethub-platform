using Identity.Application.Common.DTOs.Auth;
using Identity.Application.Features.Auth.Requests;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Organizers.RegisterOrganizer
{
    public record RegisterOrganizerCommand(RegisterOrganizerRequest Request) : IRequest<OrganizerDto>;
}
