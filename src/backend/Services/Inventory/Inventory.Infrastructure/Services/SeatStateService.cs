using Inventory.Infrastructure.Dtos;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IServices;

namespace Inventory.Infrastructure.Services
{
    public class SeatStateService : ISeatStateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisLockService _redisLockService;
        private readonly ISeatHubNotificationService _hubNotificationService;

        public SeatStateService(
            IUnitOfWork unitOfWork,
            IRedisLockService redisLockService,
            ISeatHubNotificationService hubNotificationService)
        {
            _unitOfWork = unitOfWork;
            _redisLockService = redisLockService;
            _hubNotificationService = hubNotificationService;
        }

        public async Task<IEnumerable<SeatStateDto>> GetSeatStatesAsync(Guid showtimeId, CancellationToken cancellationToken = default)
        {
            // 1. Query sold seat IDs from SQL Database
            var soldSeatIds = await _unitOfWork.ShowtimeSeatRepository.GetAllAsync<Guid>(
                filter: x => x.ShowTimeId == showtimeId && x.SeatStatus == SeatStatus.Sold,
                selector: x => x.SeatId,
                cancellation: cancellationToken
            );

            var soldSeatStrings = soldSeatIds.Select(id => id.ToString()).ToHashSet();

            // 2. Query locked seats from Redis
            var lockedSeats = await _redisLockService.GetLockedSeatsAsync(showtimeId);

            // 3. Merge data
            var result = new List<SeatStateDto>();

            // Add sold seats
            foreach (var seatId in soldSeatStrings)
            {
                result.Add(new SeatStateDto { SeatId = seatId, Status = "Sold" });
            }

            // Add locked seats (avoiding duplicates with sold seats)
            foreach (var kvp in lockedSeats)
            {
                if (!soldSeatStrings.Contains(kvp.Key))
                {
                    result.Add(new SeatStateDto { SeatId = kvp.Key, Status = kvp.Value });
                }
            }

            return result;
        }

        public async Task<bool> LockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, CancellationToken cancellationToken = default)
        {
            // 1. Verify seat is not sold in SQL DB
            var isSold = await _unitOfWork.ShowtimeSeatRepository.GetCountAsync(
                filters: x => x.ShowTimeId == showtimeId && x.SeatId == seatId && x.SeatStatus == SeatStatus.Sold,
                cancellation: cancellationToken
            ) > 0;

            if (isSold)
            {
                return false;
            }

            // 2. Try to lock in Redis (TTL: 60 seconds)
            var success = await _redisLockService.LockSeatAsync(showtimeId, seatId, userId, TimeSpan.FromSeconds(60));
            if (!success)
            {
                return false;
            }

            // 3. Notify SignalR group
            await _hubNotificationService.NotifySeatStateChangedAsync(showtimeId, seatId, "Selecting");

            return true;
        }

        public async Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, CancellationToken cancellationToken = default)
        {
            // 1. Try to unlock in Redis
            var success = await _redisLockService.UnlockSeatAsync(showtimeId, seatId, userId);
            if (!success)
            {
                return false;
            }

            // 2. Notify SignalR group
            await _hubNotificationService.NotifySeatStateChangedAsync(showtimeId, seatId, "Available");

            return true;
        }
    }
}
