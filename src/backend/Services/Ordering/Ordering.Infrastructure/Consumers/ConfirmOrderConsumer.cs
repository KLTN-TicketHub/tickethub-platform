using BuildingBlocks.Contracts.Commands.Inventory;
using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Events.Notification;
using BuildingBlocks.Contracts.Events.Order;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class ConfirmOrderConsumer : IConsumer<ConfirmOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConfirmOrderConsumer> _logger;

        public ConfirmOrderConsumer(IUnitOfWork unitOfWork, ILogger<ConfirmOrderConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ConfirmOrderCommand> context)
        {
            try
            {
                Order? order = await _unitOfWork.OrderRepository.GetByIdAsync(
                    context.Message.OrderId,
                    include: q => q.Include(o => o.OrderItems)
                );
                if (order != null && order.Status == OrderStatus.Pending)
                {
                    order.MarkAsPaid();
                    await _unitOfWork.OrderRepository.UpdateAsync(order);

                    List<ReserveSeatItemDto> seats = order.OrderItems
                        .Where(x => x.SeatId.HasValue)
                        .Select(x => new ReserveSeatItemDto
                        {
                            SeatId = x.SeatId!.Value,
                            Row = x.RowName ?? "",
                            SeatName = x.SeatName ?? "",
                            Price = x.Price
                        })
                        .ToList();

                    OrderItem? standingItem = order.OrderItems.FirstOrDefault(x => !x.SeatId.HasValue);

                    await context.Publish(new ReserveSeatsCommand
                    {
                        OrderId = order.Id,
                        ShowtimeId = order.ShowTimeId,
                        UserId = order.UserId,
                        Seats = seats,
                        TicketTypeId = standingItem?.TicketTypeId,
                        Quantity = standingItem?.Quantity ?? 0
                    });

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
                        CategoryId = order.CategoryId,
                        CategoryName = order.CategoryName,
                        ShowtimeStartAt = order.ShowtimeStartAt,
                        OrganizerId = order.OrganizerId,
                        OrganizerName = order.OrganizerName,
                        EventImage = order.EventImage,
                        ShowtimeEndAt = order.ShowtimeEndAt,
                        Items = eventItems
                    });

                    await context.Publish(new NotificationRequestedEvent
                    {
                        RecipientUserId = order.UserId,
                        Type = "OrderPaid",
                        Title = "Đặt vé thành công",
                        Message = $"Đơn hàng cho sự kiện \"{order.EventTitle}\" đã được thanh toán thành công. Vé điện tử sẽ sẵn sàng trong giây lát.",
                        LinkUrl = "/my-tickets",
                        ReferenceId = order.Id
                    });

                    await context.Publish(new NotificationRequestedEvent
                    {
                        RecipientUserId = order.OrganizerId,
                        Type = "OrderPaid",
                        Title = "Có đơn hàng mới",
                        Message = $"Sự kiện \"{order.EventTitle}\" vừa có một đơn hàng mới trị giá {order.TotalPrice:N0} VNĐ.",
                        LinkUrl = $"/organizer/events/{order.EventId}",
                        ReferenceId = order.Id
                    });

                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully confirmed order {OrderId}", context.Message.OrderId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while confirming order for OrderId: {OrderId}", context.Message.OrderId);
                throw;
            }
        }
    }
}
