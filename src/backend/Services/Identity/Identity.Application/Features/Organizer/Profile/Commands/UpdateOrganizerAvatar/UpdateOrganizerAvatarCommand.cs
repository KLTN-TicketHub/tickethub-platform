using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Features.Organizer.Profile.Commands.UpdateOrganizerAvatar
{
    public record UpdateOrganizerAvatarCommand(IFormFile File) : IRequest<string>;
}
