using Grpc.Core;
using Inventory.API.Protos;
using Inventory.Infrastructure.Interfaces.IServices;

namespace Inventory.API.Services
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly IRedisLockService _cache;
        private readonly ITicketInventoryService _ticketInventoryService;

        public InventoryGrpcService(
            IRedisLockService cache,
            ITicketInventoryService ticketInventoryService)
        {
            _cache = cache;
            _ticketInventoryService = ticketInventoryService;
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

        public override async Task<LockTicketQuantitiesResponse> LockTicketQuantities(
            LockTicketQuantitiesRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                {
                    return new LockTicketQuantitiesResponse
                    {
                        IsSuccess = false,
                        Message = "ShowtimeId không đúng định dạng Guid."
                    };
                }
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    return new LockTicketQuantitiesResponse
                    {
                        IsSuccess = false,
                        Message = "UserId không đúng định dạng Guid."
                    };
                }

                List<Guid> lockedTicketTypeIds = new List<Guid>();

                foreach (var tq in request.TicketQuantities)
                {
                    if (Guid.TryParse(tq.TicketTypeId, out var ticketTypeId))
                    {
                        bool success = await _ticketInventoryService.LockTicketsAsync(
                            showtimeId,
                            ticketTypeId,
                            userId,
                            tq.Quantity,
                            TimeSpan.FromMinutes(10),
                            context.CancellationToken);

                        if (!success)
                        {
                            foreach (var lockedId in lockedTicketTypeIds)
                            {
                                await _ticketInventoryService.UnlockTicketsAsync(showtimeId, lockedId, userId, context.CancellationToken);
                            }

                            return new LockTicketQuantitiesResponse
                            {
                                IsSuccess = false,
                                Message = $"Không đủ số lượng vé khả dụng cho loại vé {tq.TicketTypeId}."
                            };
                        }

                        lockedTicketTypeIds.Add(ticketTypeId);
                    }
                    else
                    {
                        foreach (var lockedId in lockedTicketTypeIds)
                        {
                            await _ticketInventoryService.UnlockTicketsAsync(showtimeId, lockedId, userId, context.CancellationToken);
                        }

                        return new LockTicketQuantitiesResponse
                        {
                            IsSuccess = false,
                            Message = $"TicketTypeId {tq.TicketTypeId} không đúng định dạng Guid."
                        };
                    }
                }

                return new LockTicketQuantitiesResponse { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new LockTicketQuantitiesResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }

        public override async Task<UnlockTicketQuantitiesResponse> UnlockTicketQuantities(
            UnlockTicketQuantitiesRequest request,
            ServerCallContext context)
        {
            try
            {
                if (!Guid.TryParse(request.ShowtimeId, out var showtimeId))
                {
                    return new UnlockTicketQuantitiesResponse
                    {
                        IsSuccess = false,
                        Message = "ShowtimeId không đúng định dạng Guid."
                    };
                }
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    return new UnlockTicketQuantitiesResponse
                    {
                        IsSuccess = false,
                        Message = "UserId không đúng định dạng Guid."
                    };
                }

                foreach (var ticketTypeIdStr in request.TicketTypeIds)
                {
                    if (Guid.TryParse(ticketTypeIdStr, out var ticketTypeId))
                    {
                        await _ticketInventoryService.UnlockTicketsAsync(showtimeId, ticketTypeId, userId, context.CancellationToken);
                    }
                }

                return new UnlockTicketQuantitiesResponse { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new UnlockTicketQuantitiesResponse
                {
                    IsSuccess = false,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                };
            }
        }
    }
}
