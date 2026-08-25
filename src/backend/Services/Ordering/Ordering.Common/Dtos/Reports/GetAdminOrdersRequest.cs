using BuildingBlocks.Contracts.Models.Pagination;

namespace Ordering.Common.Dtos.Reports
{
    public class GetAdminOrdersRequest : PaginatedRequest
    {
        public string? Status { get; set; }
    }
}
