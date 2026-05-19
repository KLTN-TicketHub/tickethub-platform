using Identity.Application.Common.DTOs.Auth;
using MediatR;

namespace Identity.Application.Features.Auth.Queries.GetProfile
{
    public record GetProfileQuery : IRequest<ProfileDto>
    {
    }
}