namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IInventoryService
    {
        Task<(bool IsSuccess, string Message)> UpgradeSeatLocksAsync(Guid showtimeId, List<Guid> seatIds, Guid userId);
    }
}
