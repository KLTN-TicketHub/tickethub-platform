using BuildingBlocks.Contracts.Models.Pagination;
using Inventory.Infrastructure.Entities;

namespace Inventory.Infrastructure.Dtos
{
    public class GetMyTicketsRequest : PaginatedRequest
    {
        public IssuedTicketStatus? Status { get; set; }
    }
}
