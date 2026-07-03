using BuildingBlocks.Contracts.Commands.Inventory;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IServices;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Consumers
{
    public class ReserveSeatsConsumer : IConsumer<ReserveSeatsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisLockService _redisLockService;
        private readonly ISeatHubNotificationService _hubNotificationService;
        private readonly ILogger<ReserveSeatsConsumer> _logger;

        public ReserveSeatsConsumer(
            IUnitOfWork unitOfWork,
            IRedisLockService redisLockService,
            ISeatHubNotificationService hubNotificationService,
            ILogger<ReserveSeatsConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _redisLockService = redisLockService;
            _hubNotificationService = hubNotificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReserveSeatsCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing ReserveSeatsCommand for OrderId={OrderId}", message.OrderId);

            if (message.SeatIds != null && message.SeatIds.Any())
            {
                foreach (var seatId in message.SeatIds)
                {
                    // 1. Tạo bản ghi đặt ghế trong DB với trạng thái Sold
                    var showtimeSeat = new ShowtimeSeat
                    {
                        ShowTimeId = message.ShowtimeId,
                        SeatId = seatId,
                        OrderId = message.OrderId,
                        UserId = message.UserId,
                        SeatStatus = SeatStatus.Sold
                    };
                    await _unitOfWork.ShowtimeSeatRepository.CreateAsync(showtimeSeat);

                    // 2. Unlock trong Redis (vì đã thanh toán/được lưu DB, khóa tạm không cần nữa)
                    await _redisLockService.UnlockSeatAsync(message.ShowtimeId, seatId, message.UserId);

                    // 3. Notify real-time qua SignalR
                    await _hubNotificationService.NotifySeatStateChangedAsync(message.ShowtimeId, seatId, "Sold");
                }
            }

            if (message.TicketTypeId.HasValue && message.Quantity > 0)
            {
                var inventory = await _unitOfWork.ShowtimeTicketInventoryRepository.GetOneAsync<ShowtimeTicketInventory>(
                    filter: x => x.ShowTimeId == message.ShowtimeId && x.TicketTypeId == message.TicketTypeId.Value,
                    cancellation: context.CancellationToken
                );
                if (inventory != null)
                {
                    inventory.SoldQuantity += message.Quantity;
                    await _unitOfWork.ShowtimeTicketInventoryRepository.UpdateAsync(inventory);
                }
            }

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Successfully completed ReserveSeats for Order {OrderId}", message.OrderId);
        }
    }
}
