using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Commands.Order;
using BuildingBlocks.Contracts.Events.Order;
using BuildingBlocks.Domain.Exceptions;
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

        public async Task<Guid> CheckoutAsync(CheckoutRequestDto request, Guid userId)
        {
            List<Guid> seatIds = request.Items
                .Where(x => x.SeatId.HasValue)
                .Select(x => x.SeatId!.Value)
                .ToList();

            ShowtimeSnapshot? showtimeSnapshot = await _unitOfWork.EventSnapshotRepository.GetShowtimeByIdAsync(request.ShowtimeId);
            if (showtimeSnapshot == null || showtimeSnapshot.EventSnapshot == null)
            {
                _logger.LogError(
                    "[OrderService] EventSnapshot not found for ShowtimeId={ShowtimeId}. Event has not been approved or the snapshot has not been replicated yet.",
                    request.ShowtimeId);
                throw new BusinessRuleException("Thông tin suất diễn chưa sẵn sàng. Vui lòng thử lại sau.");
            }

            EventSnapshot eventSnap = showtimeSnapshot.EventSnapshot;

            await CheckPendingOrderAsync(userId, request.ShowtimeId);

            List<ResolvedOrderItem> resolvedItems;

            if (seatIds.Any())
            {
                _logger.LogInformation(
                    "[OrderService] Reserved seat checkout, calling Catalog gRPC to validate seats for ShowtimeId={ShowtimeId}",
                    request.ShowtimeId);

                var checkoutData = await _catalogService.GetCheckoutDataAsync(
                    eventId: eventSnap.EventId,
                    showtimeId: request.ShowtimeId,
                    items: request.Items);

                if (!checkoutData.IsSuccess)
                    throw new BusinessRuleException($"Dữ liệu ghế không hợp lệ: {checkoutData.Message}");

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
                    "[OrderService] GA ticket checkout, using local EventSnapshot for ShowtimeId={ShowtimeId}",
                    request.ShowtimeId);

                resolvedItems = ValidateGAItemsFromSnapshot(request.Items, showtimeSnapshot);
            }

            if (seatIds.Any())
            {
                var (isLockSuccess, lockMessage) = await _inventoryService.UpgradeSeatLocksAsync(request.ShowtimeId, seatIds, userId);
                if (!isLockSuccess)
                    throw new BusinessRuleException($"Khóa ghế thất bại: {lockMessage}");
            }
            else
            {
                var ticketQuantities = resolvedItems
                    .Select(x => (x.TicketTypeId, x.Quantity))
                    .ToList();

                var (isLockSuccess, lockMessage) = await _inventoryService.LockTicketQuantitiesAsync(request.ShowtimeId, ticketQuantities, userId);
                if (!isLockSuccess)
                    throw new BusinessRuleException($"Khóa số lượng vé thất bại: {lockMessage}");
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

            return order.Id;
        }

        public async Task<PendingOrderDto?> GetMyPendingOrderAsync(Guid userId, Guid showtimeId)
        {
            return await GetPendingOrderAsync(userId, showtimeId);
        }

        public async Task CancelPendingOrderAsync(Guid orderId, Guid userId)
        {
            await CancelOrderAsync(orderId, userId);
        }

        private async Task CheckPendingOrderAsync(Guid userId, Guid showtimeId)
        {
            PendingOrderDto? pendingOrder = await GetPendingOrderAsync(userId, showtimeId);

            if (pendingOrder != null)
            {
                throw new BusinessRuleException("Bạn đang có một đơn hàng chưa hoàn tất thanh toán cho suất diễn này. Vui lòng hoàn tất thanh toán hoặc hủy đơn hàng đó trước khi đặt vé mới.");
            }
        }

        private async Task<PendingOrderDto?> GetPendingOrderAsync(Guid userId, Guid showtimeId)
        {
            // ExpiresAt phải khớp với Schedule(PaymentTimeout) 10 phút trong OrderBookingStateMachine
            return await _unitOfWork.OrderRepository.GetOneAsync(
                filter: x => x.UserId == userId && x.ShowTimeId == showtimeId && x.Status == OrderStatus.Pending,
                selector: x => new PendingOrderDto
                {
                    OrderId = x.Id,
                    ShowtimeId = x.ShowTimeId,
                    EventTitle = x.EventTitle,
                    TotalPrice = x.TotalPrice,
                    CreatedAt = x.CreatedAt,
                    ExpiresAt = x.CreatedAt.AddMinutes(10)
                });
        }

        private async Task CancelOrderAsync(Guid orderId, Guid userId)
        {
            Order order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, include: null)
                ?? throw new NotFoundException($"Không tìm thấy đơn hàng với ID {orderId}.");

            if (order.UserId != userId)
                throw new ForbiddenAccessException("Bạn không có quyền hủy đơn hàng này.");

            if (order.Status != OrderStatus.Pending)
                throw new BusinessRuleException("Đơn hàng này không còn ở trạng thái chờ thanh toán nên không thể hủy.");

            await _eventPublisher.PublishAsync(new CancelOrderCommand
            {
                OrderId = order.Id,
                Reason = "Người dùng huỷ đơn hàng"
            });
        }

        private List<ResolvedOrderItem> ValidateGAItemsFromSnapshot(
            List<CheckoutItemDto> requestItems, ShowtimeSnapshot showtimeSnapshot)
        {
            var result = new List<ResolvedOrderItem>();

            foreach (var item in requestItems)
            {
                TicketTypeSnapshot ttSnapshot = showtimeSnapshot.TicketTypes
                    .FirstOrDefault(t => t.TicketTypeId == item.TicketTypeId)
                    ?? throw new NotFoundException($"Loại vé {item.TicketTypeId} không tồn tại trong suất diễn.");

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

            return result;
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
