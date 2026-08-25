using BuildingBlocks.Contracts.Models.Pagination;

namespace Identity.Application.Features.Organizer.Staffs.Requests
{
    public class GetOrganizerStaffsRequest : PaginatedRequest
    {
        public string? Search { get; set; }
    }
}
