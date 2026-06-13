using Identity.Application.Features.Auth.Requests;
using MediatR;

namespace Identity.Application.Features.Auth.Commands.Organizers.ActivateOrganizer
{
    public record ActivateOrganizerCommand(ActivateOrganizerAccountRequest Request) : IRequest<Unit>;
}
