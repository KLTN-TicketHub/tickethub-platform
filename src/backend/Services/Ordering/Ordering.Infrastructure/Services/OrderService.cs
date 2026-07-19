using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Events.Order;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            IInventoryService inventoryService,
            ICatalogService catalogService,
            IEventPublisher eventPublisher,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _inventoryService = inventoryService;
            _catalogService = catalogService;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, Guid OrderId, string Message)> CheckoutAsync(CheckoutRequestDto request, Guid userId)
        {
            List<Guid> seatIds = request.Items
                .Where(x => x.SeatId.HasValue)
                .Select(x => x.SeatId!.Value)
                .ToList();

            ShowtimeSnapshot? showtimeSnapshot = await _unitOfWork.EventSnapshotRepository.GetShowtimeByIdAsync(request.ShowtimeId);
            if (showtimeSnapshot == null || showtimeSnapshot.EventSnapshot == null)
            {
                _logger.LogError(
                    "[OrderService] Không tìm thấy EventSnapshot cho ShowtimeId={ShowtimeId}. Event chưa được approve hoặc snapshot chưa được replicate.",
                    request.ShowtimeId);
                return (false, Guid.Empty, "Thông tin suất diễn chưa sẵn sàng. Vui lòng thử lại sau.");
            }

            EventSnapshot eventSnap = showtimeSnapshot.EventSnapshot;

            List<ResolvedOrderItem> resolvedItems;

            if (seatIds.Any())
            {
                _logger.LogInformation(
                    "[OrderService] Reserved seat checkout, gọi Catalog gRPC để validate ghế cho ShowtimeId={ShowtimeId}",
                    request.ShowtimeId);

                var checkoutData = await _catalogService.GetCheckoutDataAsync(
                    eventId: eventSnap.EventId,
                    showtimeId: request.ShowtimeId,
                    items: request.Items);

                if (!checkoutData.IsSuccess)
                    return (false, Guid.Empty, $"Dữ liệu ghế không hợp lệ: {checkoutData.Message}");

                resolvedItems = checkoutData.TicketItems.Select(t => new ResolvedOrderItem
                {
                    SeatId = t.SeatId,
                    SeatName = string.IsNullOrEmpty(t.SeatName) ? null : t.SeatName,
                    RowName = string.IsNullOrEmpty(t.RowName) ? null : t.RowName,
                    TicketTypeId = t.TicketTypeId,
                    TicketTypeName = t.TicketTypeName,
                    Price = t.Price,
                    Quantity = t.Quantity
                }).ToList();
            }
            else
            {
                _logger.LogInformation(
                    "[OrderService] GA ticket checkout, sử dụng EventSnapshot local cho ShowtimeId={ShowtimeId}",
                    request.ShowtimeId);

                var validationResult = ValidateGAItemsFromSnapshot(request.Items, showtimeSnapshot);
                if (!validationResult.IsSuccess)
                    return (false, Guid.Empty, validationResult.Message);

                resolvedItems = validationResult.Items;
            }

            if (seatIds.Any())
            {
                var (isLockSuccess, lockMessage) = await _inventoryService.UpgradeSeatLocksAsync(request.ShowtimeId, seatIds, userId);
                if (!isLockSuccess)
                    return (false, Guid.Empty, $"Khóa ghế thất bại: {lockMessage}");
            }

            decimal totalPrice = resolvedItems.Sum(x => x.Price * x.Quantity);

            Order order = new Order(
                userId: userId,
                customerName: request.CustomerName,
                customerEmail: request.CustomerEmail,
                customerPhone: request.CustomerPhone,
                showTimeId: request.ShowtimeId,
                eventId: eventSnap.EventId,
                eventTitle: eventSnap.EventTitle,
                categoryId: eventSnap.CategoryId,
                categoryName: eventSnap.CategoryName,
                organizerId: eventSnap.OrganizerId,
                organizerName: eventSnap.OrganizerName,
                eventImage: eventSnap.EventImage,
                showtimeStartAt: showtimeSnapshot.StartAt,
                showtimeEndAt: showtimeSnapshot.EndAt,
                totalPrice: totalPrice,
                paymentMethod: request.PaymentMethod);

            foreach (var item in resolvedItems)
            {
                order.AddOrderItem(new OrderItem(
                    seatId: item.SeatId,
                    seatName: item.SeatName,
                    rowName: item.RowName,
                    ticketTypeId: item.TicketTypeId,
                    ticketTypeName: item.TicketTypeName,
                    price: item.Price,
                    quantity: item.Quantity));
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

        private (bool IsSuccess, string Message, List<ResolvedOrderItem> Items) ValidateGAItemsFromSnapshot(
            List<CheckoutItemDto> requestItems, ShowtimeSnapshot showtimeSnapshot)
        {
            var result = new List<ResolvedOrderItem>();

            foreach (var item in requestItems)
            {
                TicketTypeSnapshot? ttSnapshot = showtimeSnapshot.TicketTypes
                    .FirstOrDefault(t => t.TicketTypeId == item.TicketTypeId);

                if (ttSnapshot == null)
                    return (false, $"Loại vé {item.TicketTypeId} không tồn tại trong suất diễn.", new());

                result.Add(new ResolvedOrderItem
                {
                    SeatId = null,
                    SeatName = null,
                    RowName = null,
                    TicketTypeId = item.TicketTypeId,
                    TicketTypeName = ttSnapshot.TicketTypeName,
                    Price = ttSnapshot.Price,
                    Quantity = item.Quantity
                });
            }

            return (true, string.Empty, result);
        }

        private class ResolvedOrderItem
        {
            public Guid? SeatId { get; set; }
            public string? SeatName { get; set; }
            public string? RowName { get; set; }
            public Guid TicketTypeId { get; set; }
            public string TicketTypeName { get; set; } = default!;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}
