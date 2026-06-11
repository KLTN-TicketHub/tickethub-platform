using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Requests;
using MediatR;

namespace Catalog.Application.Features.SeatMaps.Queries.GetSeatMapsByVenueId
{
    public record GetSeatMapsByVenueIdQuery(Guid VenueId, GetSeatMapsByVenueIdRequest Request) : IRequest<PaginatedResult<SeatMapListItemDto>>;
}
