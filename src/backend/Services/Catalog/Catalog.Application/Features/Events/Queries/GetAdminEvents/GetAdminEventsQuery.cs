using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetAdminEvents
{
    public record GetAdminEventsQuery(GetAdminEventsRequest Request) : IRequest<PaginatedResult<AdminEventListItemDto>>;
}
