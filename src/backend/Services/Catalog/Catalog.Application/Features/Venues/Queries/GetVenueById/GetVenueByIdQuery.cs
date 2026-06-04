using Catalog.Application.Common.DTOs.Venues;
using MediatR;

namespace Catalog.Application.Features.Venues.Queries.GetVenueById
{
    public record GetVenueByIdQuery(Guid Id) : IRequest<VenueDto>;
}
