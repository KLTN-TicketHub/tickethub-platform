using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.EventCancellationRequests;
using MediatR;

namespace Catalog.Application.Features.EventCancellationRequests.Queries.GetEventCancellationRequests
{
    public record GetEventCancellationRequestsQuery(int PageNumber, int PageSize) : IRequest<PaginatedResult<EventCancellationRequestDto>>;
}
