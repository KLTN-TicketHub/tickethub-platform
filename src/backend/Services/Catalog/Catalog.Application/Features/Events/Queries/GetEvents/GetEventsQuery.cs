using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Application.Common.DTOs.Events;
using Catalog.Application.Features.Events.Requests;
using MediatR;

namespace Catalog.Application.Features.Events.Queries.GetEvents
{
    public class GetEventsQuery : IRequest<PaginatedResult<EventListItemDto>>
    {
        public GetEventsRequest Request { get; set; }

        public GetEventsQuery(GetEventsRequest request)
        {
            Request = request;
        }
    }
}
