using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.UpdateVenue
{
    public record UpdateVenueCommand(Guid Id, UpdateVenueRequest Request) : IRequest<VenueDto>;
}
