namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface IRedisLockService
    {
        Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, TimeSpan ttl);

        Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId);

        Task<Dictionary<string, string>> GetLockedSeatsAsync(Guid showtimeId);

        Task<bool> LockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, int quantity, TimeSpan ttl);

        Task<bool> UnlockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId);

        Task<int> GetLockedTicketsQuantityAsync(Guid showtimeId, Guid ticketTypeId);

        Task<int> GetUserLockedTicketsQuantityAsync(Guid showtimeId, Guid ticketTypeId, Guid userId);
    }
}
