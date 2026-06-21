using BuildingBlocks.Contracts.Models.Pagination;

namespace Catalog.Application.Features.Events.Requests
{
    public class GetEventsRequest : PaginatedRequest
    {
        public Guid CategoryId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string? ProvinceCity { get; set; }
    }
}
