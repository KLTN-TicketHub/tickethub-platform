using BuildingBlocks.Contracts.Commands.Inventory;
using BuildingBlocks.Contracts.Commands.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class CancelOrderConsumer : IConsumer<CancelOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<CancelOrderCommand> context)
        {
            Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(
                context.Message.OrderId,
                include: q => q.Include(o => o.OrderItems)
            );

            if (order != null && order.Status == OrderStatus.Pending)
            {
                order.CancelOrder();
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                var seatIds = order.OrderItems
                    .Where(x => x.SeatId.HasValue)
                    .Select(x => x.SeatId!.Value)
                    .ToList();

                var standingItem = order.OrderItems.FirstOrDefault(x => !x.SeatId.HasValue);

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
            }
        }
    }
}
