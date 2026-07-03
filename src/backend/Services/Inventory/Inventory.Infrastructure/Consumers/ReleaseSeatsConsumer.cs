using BuildingBlocks.Contracts.Commands.Inventory;
using Inventory.Infrastructure.Interfaces.IServices;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Consumers
{
    public class ReleaseSeatsConsumer : IConsumer<ReleaseSeatsCommand>
    {
        private readonly IRedisLockService _redisLockService;
        private readonly ISeatHubNotificationService _hubNotificationService;
        private readonly ILogger<ReleaseSeatsConsumer> _logger;

        public ReleaseSeatsConsumer(
            IRedisLockService redisLockService,
            ISeatHubNotificationService hubNotificationService,
            ILogger<ReleaseSeatsConsumer> logger)
        {
            _redisLockService = redisLockService;
            _hubNotificationService = hubNotificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReleaseSeatsCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing ReleaseSeatsCommand for OrderId={OrderId}", message.OrderId);

            try
            {
                if (message.SeatIds != null && message.SeatIds.Any())
                {
                    foreach (var seatId in message.SeatIds)
                    {
                        // 1. Release khóa Redis ngay lập tức
                        await _redisLockService.UnlockSeatAsync(message.ShowtimeId, seatId, message.UserId);

                        // 2. Notify SignalR về trạng thái Available
                        await _hubNotificationService.NotifySeatStateChangedAsync(message.ShowtimeId, seatId, "Available");
                    }
                }
                _logger.LogInformation("Successfully completed ReleaseSeats for Order {OrderId}", message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý ReleaseSeatsCommand cho OrderId={OrderId}", message.OrderId);
                throw;
            }
        }
    }
}
