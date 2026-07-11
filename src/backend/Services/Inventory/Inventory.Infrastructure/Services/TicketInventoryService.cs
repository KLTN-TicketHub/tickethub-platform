using BuildingBlocks.Contracts.Models.Pagination;
using Inventory.Infrastructure.Dtos;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IServices;
using System.Linq.Expressions;

namespace Inventory.Infrastructure.Services
{
    public class TicketInventoryService : ITicketInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisLockService _redisLockService;

        public TicketInventoryService(
            IUnitOfWork unitOfWork,
            IRedisLockService redisLockService)
        {
            _unitOfWork = unitOfWork;
            _redisLockService = redisLockService;
        }

        public async Task<TicketInventoryStateDto?> GetTicketInventoryStateAsync(Guid showtimeId, Guid ticketTypeId, CancellationToken cancellationToken = default)
        {
            ShowtimeTicketInventory? inventory = await _unitOfWork.ShowtimeTicketInventoryRepository.GetOneAsync<ShowtimeTicketInventory>(
                filter: x => x.ShowTimeId == showtimeId && x.TicketTypeId == ticketTypeId,
                cancellation: cancellationToken
            );

            if (inventory == null)
            {
                return null;
            }

            int lockedQty = await _redisLockService.GetLockedTicketsQuantityAsync(showtimeId, ticketTypeId);

            int available = inventory.Capacity - inventory.SoldQuantity - inventory.ReservedQuantity - lockedQty;
            if (available < 0) available = 0;

            return new TicketInventoryStateDto
            {
                ShowTimeId = inventory.ShowTimeId,
                TicketTypeId = inventory.TicketTypeId,
                Capacity = inventory.Capacity,
                SoldQuantity = inventory.SoldQuantity,
                ReservedQuantity = inventory.ReservedQuantity,
                LockedQuantity = lockedQty,
                AvailableQuantity = available
            };
        }

        public async Task<bool> LockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, int quantity, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            if (quantity < 0) return false;

            if (quantity == 0)
            {
                return await UnlockTicketsAsync(showtimeId, ticketTypeId, userId, cancellationToken);
            }

            ShowtimeTicketInventory? inventory = await _unitOfWork.ShowtimeTicketInventoryRepository.GetOneAsync<ShowtimeTicketInventory>(
                filter: x => x.ShowTimeId == showtimeId && x.TicketTypeId == ticketTypeId,
                cancellation: cancellationToken
            );

            if (inventory == null)
            {
                return false;
            }

            int totalLocked = await _redisLockService.GetLockedTicketsQuantityAsync(showtimeId, ticketTypeId);
            int userLocked = await _redisLockService.GetUserLockedTicketsQuantityAsync(showtimeId, ticketTypeId, userId);

            int otherLocked = totalLocked - userLocked;
            if (otherLocked < 0) otherLocked = 0;

            int available = inventory.Capacity - inventory.SoldQuantity - inventory.ReservedQuantity - otherLocked;

            if (quantity > available)
            {
                return false;
            }

            return await _redisLockService.LockTicketsAsync(showtimeId, ticketTypeId, userId, quantity, ttl);
        }

        public async Task<bool> UnlockTicketsAsync(Guid showtimeId, Guid ticketTypeId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _redisLockService.UnlockTicketsAsync(showtimeId, ticketTypeId, userId);
        }

        public async Task<(bool Success, string Message, IssuedTicket? Ticket)> CheckInTicketAsync(string qrToken, CancellationToken cancellationToken = default)
        {
            var ticket = await _unitOfWork.IssuedTicketRepository.GetOneAsync<IssuedTicket>(
                filter: t => t.QrCodeToken == qrToken,
                cancellation: cancellationToken);

            if (ticket == null)
            {
                return (false, "Vé không hợp lệ hoặc không tồn tại.", null);
            }

            if (ticket.Status == IssuedTicketStatus.Used)
            {
                return (false, "Vé này đã được check-in trước đó.", ticket);
            }

            if (ticket.Status == IssuedTicketStatus.Cancelled)
            {
                return (false, "Vé này đã bị hủy.", ticket);
            }

            ticket.Status = IssuedTicketStatus.Used;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return (true, "Check-in thành công.", ticket);
        }

        public async Task<PaginatedResult<UserTicketDto>> GetMyTicketsAsync(Guid userId, IssuedTicketStatus? status, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var filter = (Expression<Func<IssuedTicket, bool>>)(t => t.UserId == userId && (!status.HasValue || t.Status == status.Value));

            var (tickets, totalCount) = await _unitOfWork.IssuedTicketRepository.GetPagedAsync(
                selector: t => new UserTicketDto
                {
                    IssuedTicketId = t.Id,
                    OrderId = t.OrderId,
                    EventId = t.EventId,
                    EventTitle = t.EventTitle,
                    OrganizerName = t.OrganizerName,
                    EventImage = t.EventImage,
                    QrCodeToken = t.QrCodeToken,
                    QrCodeBase64 = t.QrCodeBase64,
                    ShowtimeStartAt = t.ShowtimeStartAt,
                    SeatName = t.SeatName,
                    RowName = t.RowName,
                    TicketTypeName = t.TicketTypeName,
                    Price = t.Price,
                    Status = t.Status
                },
                filter: filter,
                orderBy: q => q.OrderByDescending(t => t.CreatedAt),
                include: null,
                pageNumber: pageIndex,
                pageSize: pageSize,
                cancellationToken: cancellationToken
            );

            return new PaginatedResult<UserTicketDto>(tickets.ToList(), totalCount, pageIndex, pageSize);
        }
    }
}
