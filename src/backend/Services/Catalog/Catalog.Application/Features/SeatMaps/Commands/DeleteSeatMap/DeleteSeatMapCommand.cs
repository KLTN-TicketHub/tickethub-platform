using MediatR;

namespace Catalog.Application.Features.SeatMaps.Commands.DeleteSeatMap
{
    public record DeleteSeatMapCommand(Guid VenueId, Guid Id) : IRequest<Unit>;
}
