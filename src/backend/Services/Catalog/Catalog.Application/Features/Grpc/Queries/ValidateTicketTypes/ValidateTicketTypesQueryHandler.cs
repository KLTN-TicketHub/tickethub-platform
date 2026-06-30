using Catalog.Application.Features.Grpc.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Grpc.Queries.ValidateTicketTypes
{
    public class ValidateTicketTypesQueryHandler : IRequestHandler<ValidateTicketTypesQuery, GrpcValidationResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateTicketTypesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GrpcValidationResult> Handle(ValidateTicketTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                // 1. Kiểm tra Showtime + Event
                var @event = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                    filter: e => e.ShowTimes.Any(st => st.Id == request.ShowtimeId) && !e.IsDeleted,
                    include: q => q.Include(e => e.ShowTimes)
                                   .ThenInclude(st => st.TicketTypes),
                    cancellation: cancellationToken);

                if (@event == null)
                    return Fail($"Sự kiện liên kết không tồn tại hoặc đã bị xóa.");

                var showtime = @event.ShowTimes.FirstOrDefault(st => st.Id == request.ShowtimeId && !st.IsDeleted);

                if (showtime == null)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không tồn tại hoặc đã bị xóa.");

                if (showtime.Status != CatalogStatus.Active)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không còn hoạt động.");

                if (showtime.StartAt <= now)
                    return Fail($"Suất diễn '{request.ShowtimeId}' đã bắt đầu hoặc đã qua.");

                if (@event.Status != EventStatus.Published)
                    return Fail("Sự kiện liên kết chưa được duyệt.");

                if (now < @event.SaleOpenAt || now > @event.SaleCloseAt)
                    return Fail("Sự kiện hiện không trong thời gian mở bán vé.");

                // 2. Kiểm tra từng TicketType
                foreach (var (ticketTypeId, quantity) in request.TicketItems)
                {
                    var ticketType = showtime.TicketTypes.FirstOrDefault(tt => tt.Id == ticketTypeId && !tt.IsDeleted);

                    if (ticketType == null)
                        return Fail($"Loại vé '{ticketTypeId}' không tồn tại, đã bị xóa hoặc không thuộc suất diễn '{request.ShowtimeId}'.");

                    if (ticketType.Status != CatalogStatus.Active)
                        return Fail($"Loại vé '{ticketType.TicketTypeName}' không còn hoạt động (trạng thái: {ticketType.Status}).");

                    if (ticketType.PublishedQuota <= 0)
                        return Fail($"Loại vé '{ticketType.TicketTypeName}' đã hết quota phát hành.");

                    if (quantity < ticketType.MinQtyQuota)
                        return Fail($"Số lượng vé '{ticketType.TicketTypeName}' tối thiểu là {ticketType.MinQtyQuota} vé mỗi lần đặt.");

                    if (quantity > ticketType.MaxQtyQuota)
                        return Fail($"Số lượng vé '{ticketType.TicketTypeName}' tối đa là {ticketType.MaxQtyQuota} vé mỗi lần đặt.");
                }

                return new GrpcValidationResult(true, "Validation TicketType thành công.");
            }
            catch (Exception ex)
            {
                return Fail($"Lỗi hệ thống: {ex.Message}");
            }
        }

        private static GrpcValidationResult Fail(string message) => new(false, message);
    }
}
