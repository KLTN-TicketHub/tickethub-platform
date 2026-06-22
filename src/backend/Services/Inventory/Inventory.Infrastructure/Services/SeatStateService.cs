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
            IEnumerable<Guid> soldSeatIds = await _unitOfWork.ShowtimeSeatRepository.GetAllAsync(
                filter: x => x.ShowTimeId == showtimeId && x.SeatStatus == SeatStatus.Sold,
                selector: x => x.SeatId,
                cancellation: cancellationToken
            );

            IEnumerable<string> soldSeatStrings = soldSeatIds.Select(id => id.ToString()).ToHashSet();

            Dictionary<string, string> lockedSeats = await _redisLockService.GetLockedSeatsAsync(showtimeId);

            List<SeatStateDto> result = new List<SeatStateDto>();

            foreach (var seatId in soldSeatStrings)
            {
                result.Add(new SeatStateDto { SeatId = seatId, Status = "Sold" });
            }

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
            bool isSold = await _unitOfWork.ShowtimeSeatRepository.GetCountAsync(
                filters: x => x.ShowTimeId == showtimeId && x.SeatId == seatId && x.SeatStatus == SeatStatus.Sold,
                cancellation: cancellationToken
            ) > 0;

            if (isSold)
            {
                return false;
            }

            bool success = await _redisLockService.LockSeatAsync(showtimeId, seatId, userId, TimeSpan.FromSeconds(60));
            if (!success)
            {
                return false;
            }

            await _hubNotificationService.NotifySeatStateChangedAsync(showtimeId, seatId, "Selecting");

            return true;
        }

        public async Task<bool> UnlockSeatAsync(Guid showtimeId, Guid seatId, Guid userId, CancellationToken cancellationToken = default)
        {
            bool success = await _redisLockService.UnlockSeatAsync(showtimeId, seatId, userId);
            if (!success)
            {
                return false;
            }

            await _hubNotificationService.NotifySeatStateChangedAsync(showtimeId, seatId, "Available");

            return true;
        }
    }
}
