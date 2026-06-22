namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface IRedisLockService
    {
        Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, TimeSpan ttl);

        Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId);

        Task<Dictionary<string, string>> GetLockedSeatsAsync(Guid showtimeId);
    }
}
