using BuildingBlocks.Contracts.Models.Pagination;

namespace Catalog.Application.Features.Events.Requests
{
    public class GetEventsForModeratorRequest : PaginatedRequest
    {
        public EventStatusForModerator? Status { get; set; }
    }
    public enum EventStatusForModerator
    {
        PendingApproval,
        Published,
        Rejected
    }
}
