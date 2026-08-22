using Catalog.Application.Common.DTOs.Profiles;
using MediatR;

namespace Catalog.Application.Features.Organizers.Queries.GetFeaturedOrganizers
{
    public record GetFeaturedOrganizersQuery(int Count) : IRequest<List<FeaturedOrganizerDto>>;
}
