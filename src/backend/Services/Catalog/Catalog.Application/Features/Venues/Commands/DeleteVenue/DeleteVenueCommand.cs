using MediatR;

namespace Catalog.Application.Features.Venues.Commands.DeleteVenue
{
    public record DeleteVenueCommand(Guid Id) : IRequest<Unit>;
}