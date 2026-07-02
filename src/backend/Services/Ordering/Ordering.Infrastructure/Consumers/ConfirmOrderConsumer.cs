using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Events.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class ConfirmOrderConsumer : IConsumer<ConfirmOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmOrderConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<ConfirmOrderCommand> context)
        {
            Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(
                context.Message.OrderId,
                include: q => q.Include(o => o.OrderItems)
            );
            if (order != null && order.Status == OrderStatus.Pending)
            {
                order.MarkAsPaid();
                await _unitOfWork.OrderRepository.UpdateAsync(order);

                List<OrderPaidItemDto> eventItems = order.OrderItems.Select(x => new OrderPaidItemDto
                {
                    SeatId = x.SeatId,
                    SeatName = x.SeatName,
                    RowName = x.RowName,
                    TicketTypeId = x.TicketTypeId,
                    TicketTypeName = x.TicketTypeName,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList();

                await context.Publish(new OrderPaidEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    TotalPrice = order.TotalPrice,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    ShowTimeId = order.ShowTimeId,
                    EventId = order.EventId,
                    EventTitle = order.EventTitle,
                    ShowtimeStartAt = order.ShowtimeStartAt,
                    Items = eventItems
                });

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
