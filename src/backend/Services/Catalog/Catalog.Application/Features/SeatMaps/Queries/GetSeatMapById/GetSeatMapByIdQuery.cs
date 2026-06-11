using Catalog.Application.Common.DTOs.SeatMaps;
using MediatR;

namespace Catalog.Application.Features.SeatMaps.Queries.GetSeatMapById
{
    public record GetSeatMapByIdQuery(Guid VenueId, Guid Id) : IRequest<SeatMapDto>;
}
