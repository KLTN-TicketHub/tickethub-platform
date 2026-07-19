using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Grpc.Queries.GetCheckoutData
{
    public class GetCheckoutDataQueryHandler : IRequestHandler<GetCheckoutDataQuery, GetCheckoutDataResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCheckoutDataQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCheckoutDataResult> Handle(GetCheckoutDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                DateTime now = DateTime.UtcNow;

                Event? @event = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                    filter: e => e.Id == request.EventId && !e.IsDeleted,
                    include: q => q.Include(e => e.ShowTimes)
                                   .ThenInclude(st => st.TicketTypes)
                                   .Include(e => e.Organizer),
                    cancellation: cancellationToken);

                if (@event == null)
                    return GetCheckoutDataResult.Fail($"Sự kiện với Id '{request.EventId}' không tồn tại hoặc đã bị xóa.");

                if (@event.Status != EventStatus.Published)
                    return GetCheckoutDataResult.Fail($"Sự kiện '{@event.Title}' chưa được duyệt hoặc đã đóng (trạng thái: {@event.Status}).");

                if (now < @event.SaleOpenAt)
                    return GetCheckoutDataResult.Fail($"Sự kiện '{@event.Title}' chưa mở bán vé (mở bán lúc: {@event.SaleOpenAt:dd/MM/yyyy HH:mm} UTC).");

                if (now > @event.SaleCloseAt)
                    return GetCheckoutDataResult.Fail($"Sự kiện '{@event.Title}' đã đóng bán vé (đóng lúc: {@event.SaleCloseAt:dd/MM/yyyy HH:mm} UTC).");

                var showtime = @event.ShowTimes.FirstOrDefault(st => st.Id == request.ShowtimeId && !st.IsDeleted);

                if (showtime == null)
                    return GetCheckoutDataResult.Fail($"Suất diễn '{request.ShowtimeId}' không tồn tại, đã bị xóa hoặc không thuộc sự kiện '{request.EventId}'.");

                if (showtime.Status != CatalogStatus.Active)
                    return GetCheckoutDataResult.Fail($"Suất diễn '{request.ShowtimeId}' không còn hoạt động (trạng thái: {showtime.Status}).");

                var result = new GetCheckoutDataResult
                {
                    IsSuccess = true,
                    Message = "Thành công"
                };

                var seatIds = request.Items.Where(x => x.SeatId.HasValue).Select(x => x.SeatId!.Value).ToList();
                var seats = new Dictionary<Guid, Seat>();
                if (seatIds.Any())
                {
                    if (@event.SeatMapId == null)
                        return GetCheckoutDataResult.Fail($"Sự kiện '{@event.Title}' không có sơ đồ ghế, không thể chọn ghế cụ thể.");

                    var seatEntities = await _unitOfWork.SeatRepository.GetAllAsync<Seat>(
                        filter: s => seatIds.Contains(s.Id) && !s.IsDeleted,
                        include: q => q.Include(s => s.Row)
                                       .ThenInclude(r => r!.Zone),
                        cancellation: cancellationToken);

                    seats = seatEntities.ToDictionary(s => s.Id);
                }

                foreach (var item in request.Items)
                {
                    var ticketType = showtime.TicketTypes.FirstOrDefault(tt => tt.Id == item.TicketTypeId && !tt.IsDeleted);

                    if (ticketType == null)
                        return GetCheckoutDataResult.Fail($"Loại vé '{item.TicketTypeId}' không tồn tại, đã bị xóa hoặc không thuộc suất diễn '{request.ShowtimeId}'.");

                    if (ticketType.Status != CatalogStatus.Active)
                        return GetCheckoutDataResult.Fail($"Loại vé '{ticketType.TicketTypeName}' không còn hoạt động (trạng thái: {ticketType.Status}).");

                    if (item.Quantity < ticketType.MinQtyQuota)
                        return GetCheckoutDataResult.Fail($"Số lượng vé '{ticketType.TicketTypeName}' phải tối thiểu {ticketType.MinQtyQuota} vé mỗi lần đặt.");

                    if (item.Quantity > ticketType.MaxQtyQuota)
                        return GetCheckoutDataResult.Fail($"Số lượng vé '{ticketType.TicketTypeName}' không được vượt quá {ticketType.MaxQtyQuota} vé mỗi lần đặt.");

                    if (item.SeatId.HasValue)
                    {
                        if (!seats.TryGetValue(item.SeatId.Value, out var seat))
                            return GetCheckoutDataResult.Fail($"Ghế '{item.SeatId.Value}' không tồn tại hoặc đã bị xóa.");

                        if (seat.LayoutStatus != SeatLayoutStatus.Available)
                            return GetCheckoutDataResult.Fail($"Ghế '{seat.SeatName}' hiện không khả dụng (trạng thái: {seat.LayoutStatus}).");

                        var row = seat.Row;
                        if (row == null || row.IsDeleted)
                            return GetCheckoutDataResult.Fail($"Hàng ghế của ghế '{seat.SeatName}' không tồn tại hoặc đã bị xóa.");

                        if (row.Status != CatalogStatus.Active)
                            return GetCheckoutDataResult.Fail($"Hàng ghế '{row.RowName}' của ghế '{seat.SeatName}' không còn hoạt động.");

                        var zone = row.Zone;
                        if (zone == null || zone.IsDeleted)
                            return GetCheckoutDataResult.Fail($"Khu vực của ghế '{seat.SeatName}' không tồn tại hoặc đã bị xóa.");

                        if (zone.Status != CatalogStatus.Active)
                            return GetCheckoutDataResult.Fail($"Khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}' không còn hoạt động.");

                        if (!zone.IsSalable)
                            return GetCheckoutDataResult.Fail($"Khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}' không được phép bán vé.");

                        if (zone.SeatMapId != @event.SeatMapId!.Value)
                            return GetCheckoutDataResult.Fail($"Ghế '{seat.SeatName}' (khu vực '{zone.ZoneName}') không thuộc sơ đồ ghế của sự kiện này.");

                        if (ticketType.ZoneId.HasValue && ticketType.ZoneId.Value != zone.Id)
                            return GetCheckoutDataResult.Fail($"Loại vé '{ticketType.TicketTypeName}' không áp dụng cho khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}'.");

                        result.TicketItems.Add(new ValidatedTicketItemResult
                        {
                            SeatId = seat.Id,
                            TicketTypeId = ticketType.Id,
                            TicketTypeName = ticketType.TicketTypeName,
                            Price = ticketType.Price,
                            SeatName = seat.SeatName,
                            RowName = row.RowName,
                            Quantity = item.Quantity
                        });
                    }
                    else
                    {
                        if (ticketType.ZoneId.HasValue)
                        {
                            var zone = await _unitOfWork.ZoneRepository.GetByIdAsync(ticketType.ZoneId.Value, include: null);
                            if (zone != null && zone.IsReservingSeat)
                                return GetCheckoutDataResult.Fail($"Loại vé '{ticketType.TicketTypeName}' yêu cầu phải chọn ghế cụ thể.");
                        }

                        result.TicketItems.Add(new ValidatedTicketItemResult
                        {
                            SeatId = null,
                            TicketTypeId = ticketType.Id,
                            TicketTypeName = ticketType.TicketTypeName,
                            Price = ticketType.Price,
                            SeatName = null,
                            RowName = null,
                            Quantity = item.Quantity
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return GetCheckoutDataResult.Fail($"Lỗi hệ thống khi lấy dữ liệu checkout: {ex.Message}");
            }
        }
    }
}
