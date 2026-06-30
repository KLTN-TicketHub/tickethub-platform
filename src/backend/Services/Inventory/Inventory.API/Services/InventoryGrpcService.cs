using Grpc.Core;
using Inventory.API.Protos;
using Inventory.Infrastructure.Interfaces.IServices;

namespace Inventory.API.Services
{
    public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
    {
        private readonly IRedisLockService _cache;
        private readonly ITicketInventoryService _ticketInventoryService;
        private readonly ICatalogService _catalogService;

        public InventoryGrpcService(
            IRedisLockService cache,
            ITicketInventoryService ticketInventoryService,
            ICatalogService catalogService)
        {
            _cache = cache;
            _ticketInventoryService = ticketInventoryService;
            _catalogService = catalogService;
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

                var seatIdsParsed = new List<Guid>();
                foreach (var seatIdStr in request.SeatIds)
                {
                    if (!Guid.TryParse(seatIdStr, out var seatId))
                    {
                        return new UpgradeSeatLocksResponse
                        {
                            IsSuccess = false,
                            Message = $"SeatId '{seatIdStr}' không đúng định dạng Guid."
                        };
                    }
                    seatIdsParsed.Add(seatId);
                }

                var (isValidCatalog, catalogMessage) = await _catalogService.ValidateSeatLockAsync(
                    showtimeId, seatIdsParsed);

                if (!isValidCatalog)
                {
                    return new UpgradeSeatLocksResponse
                    {
                        IsSuccess = false,
                        Message = $"Catalog validation thất bại: {catalogMessage}"
                    };
                }

                foreach (var seatId in seatIdsParsed)
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
                            Message = $"Ghế '{seatId}' không thể nâng cấp khóa (có thể đã hết hạn giữ chỗ hoặc bị người khác chọn)."
                        };
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

                var ticketItems = new List<(Guid TicketTypeId, int Quantity)>();
                foreach (var tq in request.TicketQuantities)
                {
                    if (!Guid.TryParse(tq.TicketTypeId, out var ticketTypeId))
                    {
                        return new LockTicketQuantitiesResponse
                        {
                            IsSuccess = false,
                            Message = $"TicketTypeId '{tq.TicketTypeId}' không đúng định dạng Guid."
                        };
                    }
                    ticketItems.Add((ticketTypeId, tq.Quantity));
                }

                var (isValidCatalog, catalogMessage) = await _catalogService.ValidateTicketTypesAsync(
                    showtimeId, ticketItems);

                if (!isValidCatalog)
                {
                    return new LockTicketQuantitiesResponse
                    {
                        IsSuccess = false,
                        Message = $"Catalog validation thất bại: {catalogMessage}"
                    };
                }

                List<Guid> lockedTicketTypeIds = new List<Guid>();

                foreach (var (ticketTypeId, quantity) in ticketItems)
                {
                    bool success = await _ticketInventoryService.LockTicketsAsync(
                        showtimeId,
                        ticketTypeId,
                        userId,
                        quantity,
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
                            Message = $"Không đủ số lượng vé khả dụng cho loại vé '{ticketTypeId}'."
                        };
                    }

                    lockedTicketTypeIds.Add(ticketTypeId);
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
