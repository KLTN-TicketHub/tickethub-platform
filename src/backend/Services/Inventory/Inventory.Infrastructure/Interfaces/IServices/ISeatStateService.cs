using Inventory.Infrastructure.Dtos;

namespace Inventory.Infrastructure.Interfaces.IServices
{
    public interface ISeatStateService
    {
        Task<IEnumerable<SeatStateDto>> GetSeatStatesAsync(Guid showtimeId, CancellationToken cancellationToken = default);
        Task LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, CancellationToken cancellationToken = default);
        Task UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, CancellationToken cancellationToken = default);
    }
}
