using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Venues;
using Catalog.Application.Features.Venues.Requests;
using MediatR;

namespace Catalog.Application.Features.Venues.Queries.GetVenues
{
    public record GetVenuesQuery(GetVenuesRequest Request) : IRequest<PaginatedResult<VenueListItemDto>>;
}
