using BuildingBlocks.Contracts.Models.Pagination;
using Inventory.Infrastructure.Dtos;
using Inventory.Infrastructure.Entities;

namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface ITicketInventoryService
    {
        Task<TicketInventoryStateDto?> GetTicketInventoryStateAsync(Guid showtimeId, Guid ticketTypeId, CancellationToken cancellationToken = default);
        Task<bool> LockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, int quantity, TimeSpan ttl, CancellationToken cancellationToken = default);
        Task<bool> UnlockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message, Inventory.Infrastructure.Entities.IssuedTicket? Ticket)> CheckInTicketAsync(string qrToken, CancellationToken cancellationToken = default);
        Task<PaginatedResult<UserTicketDto>> GetMyTicketsAsync(Guid userId, IssuedTicketStatus? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default);
    }
}
