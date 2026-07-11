using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Order;
using Ordering.Common.Dtos;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;
using Ordering.Infrastructure.Interfaces.IServices;

namespace Ordering.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryService _inventoryService;
        private readonly ICatalogService _catalogService;
        private readonly IEventPublisher _eventPublisher;

        public OrderService(
            IUnitOfWork unitOfWork,
            IInventoryService inventoryService,
            ICatalogService catalogService,
            IEventPublisher eventPublisher)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _catalogService = catalogService;
            _eventPublisher = eventPublisher;
        }

        public async Task<(bool IsSuccess, Guid OrderId, string Message)> CheckoutAsync(CheckoutRequestDto request, Guid userId)
        {
            List<Guid> seatIds = request.Items
                .Where(x => x.SeatId.HasValue)
                .Select(x => x.SeatId!.Value)
                .ToList();

            var checkoutData = await _catalogService.GetCheckoutDataAsync(
                eventId: request.EventId,
                showtimeId: request.ShowtimeId,
                items: request.Items);

            if (!checkoutData.IsSuccess)
                return (false, Guid.Empty, $"Dữ liệu không hợp lệ từ Catalog: {checkoutData.Message}");

            if (seatIds.Any())
            {
                var (isLockSuccess, lockMessage) = await _inventoryService.UpgradeSeatLocksAsync(request.ShowtimeId, seatIds, userId);
                if (!isLockSuccess)
                    return (false, Guid.Empty, $"Khóa ghế thất bại: {lockMessage}");
            }

            decimal totalPrice = checkoutData.TicketItems.Sum(x => x.Price * x.Quantity);

            Order order = new Order(
                userId: userId,
                customerName: request.CustomerName,
                customerEmail: request.CustomerEmail,
                customerPhone: request.CustomerPhone,
                showTimeId: request.ShowtimeId,
                eventId: request.EventId,
                eventTitle: checkoutData.EventTitle,
                organizerId: checkoutData.OrganizerId,
                organizerName: checkoutData.OrganizerName,
                eventImage: checkoutData.EventImage,
                showtimeStartAt: checkoutData.ShowtimeStartAt,
                showtimeEndAt: checkoutData.ShowtimeEndAt,
                totalPrice: totalPrice,
                paymentMethod: request.PaymentMethod
            );

            foreach (var item in checkoutData.TicketItems)
            {
                var orderItem = new OrderItem(
                    seatId: item.SeatId,
                    seatName: item.SeatName,
                    rowName: item.RowName,
                    ticketTypeId: item.TicketTypeId,
                    ticketTypeName: item.TicketTypeName,
                    price: item.Price,
                    quantity: item.Quantity
                );
                order.AddOrderItem(orderItem);
            }

            await _unitOfWork.OrderRepository.CreateAsync(order);

            CheckoutStartedEvent checkoutEvent = new CheckoutStartedEvent
            {
                OrderId = order.Id,
                UserId = userId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                ShowtimeId = request.ShowtimeId,
                TotalPrice = totalPrice,
                PaymentMethod = request.PaymentMethod,
                Items = request.Items.Select(x => new CheckoutStartedItemDto
                {
                    SeatId = x.SeatId,
                    TicketTypeId = x.TicketTypeId,
                    Quantity = x.Quantity
                }).ToList()
            };

            await _eventPublisher.PublishAsync(checkoutEvent);

            await _unitOfWork.SaveChangesAsync();

            return (true, order.Id, "Đơn hàng đã được tạo thành công.");
        }
    }
}
