using BuildingBlocks.Contracts.Models.Pagination;
using Catalog.Domain.Enums;

namespace Catalog.Application.Features.Events.Requests
{
    public class GetAdminEventsRequest : PaginatedRequest
    {
        public string? Search { get; set; }

        public EventStatus? Status { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
