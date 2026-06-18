using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events.Requests
{
    public class GetEventsForOrganizerRequest : PaginatedRequest
    {
        public EventStatus? Status { get; set; }
    }
}
