using Grpc.Core;
using Inventory.API.Protos;
using Inventory.Infrastructure.Interfaces.IServices;

namespace Inventory.API.Services
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly IRedisLockService _cache;

        public InventoryGrpcService(IRedisLockService cache)
        {
            _cache = cache;
        }

        public override async Task<UpgradeSeatLocksResponse> UpgradeSeatLocks(
            UpgradeSeatLocksRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                {
                    return new UpgradeSeatLocksResponse
                    {
                        IsSuccess = false,
                        Message = "ShowtimeId không đúng định dạng Guid."
                    };
                }
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    return new UpgradeSeatLocksResponse
                    {
                        IsSuccess = false,
                        Message = "UserId không đúng định dạng Guid."
                    };
                }
                foreach (var seatIdStr in request.SeatIds)
                {
                    if (Guid.TryParse(seatIdStr, out var seatId))
                    {
                        var locked = await _cache.LockSeatAsync(
                            showtimeId,
                            seatId,
                            userId,
                            TimeSpan.FromMinutes(10));
                        if (!locked)
                        {
                            return new UpgradeSeatLocksResponse
                            {
                                IsSuccess = false,
                                Message = $"Ghế {seatIdStr} không thể nâng cấp khóa (có thể đã hết hạn giữ chỗ hoặc bị người khác chọn)."
                            };
                        }
                    }
                }
                return new UpgradeSeatLocksResponse { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new UpgradeSeatLocksResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi xử lý hệ thống: {ex.Message}"
                };
            }
        }
    }
}
