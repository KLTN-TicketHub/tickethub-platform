using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using MediatR;

namespace Catalog.Application.Features.Venues.Commands.CreateVenue
{
    public record CreateVenueCommand(CreateVenueRequest Request) : IRequest<VenueDto>;
}
