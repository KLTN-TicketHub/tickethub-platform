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

            try
            {
                if (message.Seats != null && message.Seats.Any())
                {
                    foreach (var seat in message.Seats)
                    {
                        var showtimeSeat = new ShowtimeSeat
                        {
                            ShowTimeId = message.ShowtimeId,
                            SeatId = seat.SeatId,
                            OrderId = message.OrderId,
                            UserId = message.UserId,
                            SeatStatus = SeatStatus.Sold,
                            Price = seat.Price,
                            Row = seat.Row,
                            SeatName = seat.SeatName
                        };
                        await _unitOfWork.ShowtimeSeatRepository.CreateAsync(showtimeSeat);

                        await _redisLockService.UnlockSeatAsync(message.ShowtimeId, seat.SeatId, message.UserId, force: true);

                        await _hubNotificationService.NotifySeatStateChangedAsync(message.ShowtimeId, seat.SeatId, "Sold");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ReserveSeatsCommand cho OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
