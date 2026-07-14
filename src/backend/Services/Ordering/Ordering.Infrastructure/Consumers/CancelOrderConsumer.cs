using BuildingBlocks.Contracts.Commands.Inventory;
using BuildingBlocks.Contracts.Commands.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class CancelOrderConsumer : IConsumer<CancelOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CancelOrderConsumer> _logger;

        public CancelOrderConsumer(IUnitOfWork unitOfWork, ILogger<CancelOrderConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CancelOrderCommand> context)
        {
            try
            {
                Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(
                    context.Message.OrderId,
                    include: q => q.Include(o => o.OrderItems)
                );

                if (order != null && order.Status == OrderStatus.Pending)
                {
                    order.CancelOrder();
                    await _unitOfWork.OrderRepository.UpdateAsync(order);

                    List<Guid> seatIds = order.OrderItems
                        .Where(x => x.SeatId.HasValue)
                        .Select(x => x.SeatId!.Value)
                        .ToList();

                    OrderItem? standingItem = order.OrderItems.FirstOrDefault(x => !x.SeatId.HasValue);

                    await context.Publish(new ReleaseSeatsCommand
                    {
                        OrderId = order.Id,
                        ShowtimeId = order.ShowTimeId,
                        UserId = order.UserId,
                        SeatIds = seatIds,
                        TicketTypeId = standingItem?.TicketTypeId,
                        Quantity = standingItem?.Quantity ?? 0
                    });

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully cancelled order {OrderId}", context.Message.OrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra khi CancelOrder cho OrderId: {OrderId}", context.Message.OrderId);
                throw;
            }
        }
    }
}
