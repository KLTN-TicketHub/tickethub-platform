using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Requests;
using MediatR;

namespace Catalog.Application.Features.SeatMaps.Commands.CreateSeatMap
{
    public record CreateSeatMapCommand(Guid VenueId, CreateSeatMapRequest Request) : IRequest<SeatMapDto>;
}
