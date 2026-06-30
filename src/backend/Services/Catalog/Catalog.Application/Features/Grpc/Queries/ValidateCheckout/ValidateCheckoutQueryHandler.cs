using Catalog.Application.Features.Grpc.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Grpc.Queries.ValidateCheckout
{
    public class ValidateCheckoutQueryHandler : IRequestHandler<ValidateCheckoutQuery, GrpcValidationResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateCheckoutQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GrpcValidationResult> Handle(ValidateCheckoutQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                var @event = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                    filter: e => e.Id == request.EventId && !e.IsDeleted,
                    include: q => q.Include(e => e.ShowTimes)
                                   .ThenInclude(st => st.TicketTypes),
                    cancellation: cancellationToken);

                if (@event == null)
                    return Fail($"Sự kiện với Id '{request.EventId}' không tồn tại hoặc đã bị xóa.");

                if (@event.Status != EventStatus.Published)
                    return Fail($"Sự kiện '{@event.Title}' chưa được duyệt hoặc đã đóng (trạng thái: {@event.Status}).");

                if (now < @event.SaleOpenAt)
                    return Fail($"Sự kiện '{@event.Title}' chưa mở bán vé (mở bán lúc: {@event.SaleOpenAt:dd/MM/yyyy HH:mm} UTC).");

                if (now > @event.SaleCloseAt)
                    return Fail($"Sự kiện '{@event.Title}' đã đóng bán vé (đóng lúc: {@event.SaleCloseAt:dd/MM/yyyy HH:mm} UTC).");

                var showtime = @event.ShowTimes.FirstOrDefault(st => st.Id == request.ShowtimeId && !st.IsDeleted);

                if (showtime == null)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không tồn tại, đã bị xóa hoặc không thuộc sự kiện '{request.EventId}'.");

                if (showtime.Status != CatalogStatus.Active)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không còn hoạt động (trạng thái: {showtime.Status}).");

                if (showtime.StartAt <= now)
                    return Fail($"Suất diễn '{request.ShowtimeId}' đã bắt đầu hoặc đã qua, không thể đặt vé.");

                foreach (var (ticketTypeId, quantity) in request.TicketItems)
                {
                    var ticketType = showtime.TicketTypes.FirstOrDefault(tt => tt.Id == ticketTypeId && !tt.IsDeleted);

                    if (ticketType == null)
                        return Fail($"Loại vé '{ticketTypeId}' không tồn tại, đã bị xóa hoặc không thuộc suất diễn '{request.ShowtimeId}'.");

                    if (ticketType.Status != CatalogStatus.Active)
                        return Fail($"Loại vé '{ticketType.TicketTypeName}' không còn hoạt động (trạng thái: {ticketType.Status}).");

                    if (quantity < ticketType.MinQtyQuota)
                        return Fail($"Số lượng vé '{ticketType.TicketTypeName}' phải tối thiểu {ticketType.MinQtyQuota} vé mỗi lần đặt.");

                    if (quantity > ticketType.MaxQtyQuota)
                        return Fail($"Số lượng vé '{ticketType.TicketTypeName}' không được vượt quá {ticketType.MaxQtyQuota} vé mỗi lần đặt.");
                }

                if (request.SeatIds.Any())
                {
                    if (@event.SeatMapId == null)
                        return Fail($"Sự kiện '{@event.Title}' không có sơ đồ ghế, không thể chọn ghế cụ thể.");

                    var seats = await _unitOfWork.SeatRepository.GetAllAsync<Seat>(
                        filter: s => request.SeatIds.Contains(s.Id) && !s.IsDeleted,
                        include: q => q.Include(s => s.Row)
                                       .ThenInclude(r => r!.Zone),
                        cancellation: cancellationToken);

                    var seatMap = seats.ToDictionary(s => s.Id);

                    foreach (var seatId in request.SeatIds)
                    {
                        if (!seatMap.TryGetValue(seatId, out var seat))
                            return Fail($"Ghế '{seatId}' không tồn tại hoặc đã bị xóa.");

                        if (seat.LayoutStatus != SeatLayoutStatus.Available)
                            return Fail($"Ghế '{seat.SeatName}' hiện không khả dụng (trạng thái: {seat.LayoutStatus}).");

                        var row = seat.Row;
                        if (row == null || row.IsDeleted)
                            return Fail($"Hàng ghế của ghế '{seat.SeatName}' không tồn tại hoặc đã bị xóa.");

                        if (row.Status != CatalogStatus.Active)
                            return Fail($"Hàng ghế '{row.RowName}' của ghế '{seat.SeatName}' không còn hoạt động.");

                        var zone = row.Zone;
                        if (zone == null || zone.IsDeleted)
                            return Fail($"Khu vực của ghế '{seat.SeatName}' không tồn tại hoặc đã bị xóa.");

                        if (zone.Status != CatalogStatus.Active)
                            return Fail($"Khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}' không còn hoạt động.");

                        if (!zone.IsSalable)
                            return Fail($"Khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}' không được phép bán vé.");

                        if (zone.SeatMapId != @event.SeatMapId.Value)
                            return Fail($"Ghế '{seat.SeatName}' (khu vực '{zone.ZoneName}') không thuộc sơ đồ ghế của sự kiện này.");
                    }
                }

                return new GrpcValidationResult(true, "Validation checkout thành công.");
            }
            catch (Exception ex)
            {
                return Fail($"Lỗi hệ thống khi validate checkout: {ex.Message}");
            }
        }

        private static GrpcValidationResult Fail(string message) => new(false, message);
    }
}
