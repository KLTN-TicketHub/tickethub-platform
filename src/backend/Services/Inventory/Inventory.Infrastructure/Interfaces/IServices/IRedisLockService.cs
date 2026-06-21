namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface IRedisLockService
    {
        Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, string userId, TimeSpan ttl);

        Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, string userId);

        Task<Dictionary<string, string>> GetLockedSeatsAsync(Guid showtimeId);
    }
}
