using Inventory.Infrastructure.Dtos;

namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface ITicketInventoryService
    {
        Task<TicketInventoryStateDto?> GetTicketInventoryStateAsync(Guid showtimeId, Guid ticketTypeId, CancellationToken cancellationToken = default);
        Task<bool> LockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, int quantity, TimeSpan ttl, CancellationToken cancellationToken = default);
        Task<bool> UnlockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, CancellationToken cancellationToken = default);
    }
}
