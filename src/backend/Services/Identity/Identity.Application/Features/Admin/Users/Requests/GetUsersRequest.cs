using BuildingBlocks.Contracts.Models.Pagination;

namespace Identity.Application.Features.Admin.Users.Requests
{
    public class GetUsersRequest : PaginatedRequest
    {
        public string? Role { get; set; }

        public bool? IsLocked { get; set; }
    }
}
