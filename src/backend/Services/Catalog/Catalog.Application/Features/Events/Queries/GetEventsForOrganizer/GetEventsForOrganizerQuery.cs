using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEventsForOrganizer
{
    public record GetEventsForOrganizerQuery(GetEventsForOrganizerRequest Request) : IRequest<PaginatedResult<OrganizerEventListItemDto>>;
}
