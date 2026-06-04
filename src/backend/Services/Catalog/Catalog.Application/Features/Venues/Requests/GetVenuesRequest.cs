using BuildingBlocks.Contracts.Models.Pagination;

namespace Catalog.Application.Features.Venues.Requests
{
    public class GetVenuesRequest : PaginatedRequest
    {
        public string? ProvinceCity { get; set; }

        public string? District { get; set; }
    }
}
