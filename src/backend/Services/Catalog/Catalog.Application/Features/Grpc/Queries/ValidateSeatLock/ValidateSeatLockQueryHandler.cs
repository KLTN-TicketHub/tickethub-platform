using Catalog.Application.Features.Grpc.Common;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Features.Grpc.Queries.ValidateSeatLock
{
    public class ValidateSeatLockQueryHandler : IRequestHandler<ValidateSeatLockQuery, GrpcValidationResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ValidateSeatLockQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GrpcValidationResult> Handle(ValidateSeatLockQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                var @event = await _unitOfWork.EventRepository.GetOneUntrackedAsync<Event>(
                    filter: e => e.ShowTimes.Any(st => st.Id == request.ShowtimeId) && !e.IsDeleted,
                    include: q => q.Include(e => e.ShowTimes),
                    cancellation: cancellationToken);

                if (@event == null)
                    return Fail($"Sự kiện liên kết với suất diễn không tồn tại hoặc đã bị xóa.");

                var showtime = @event.ShowTimes.FirstOrDefault(st => st.Id == request.ShowtimeId && !st.IsDeleted);

                if (showtime == null)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không tồn tại hoặc đã bị xóa.");

                if (showtime.Status != CatalogStatus.Active)
                    return Fail($"Suất diễn '{request.ShowtimeId}' không còn hoạt động.");

                if (@event.Status != EventStatus.Published)
                    return Fail($"Sự kiện '{@event.Title}' chưa được duyệt hoặc đã đóng.");

                if (@event.SeatMapId == null)
                    return Fail($"Sự kiện '{@event.Title}' không có sơ đồ ghế.");

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
                    if (row == null || row.IsDeleted || row.Status != CatalogStatus.Active)
                        return Fail($"Hàng ghế của ghế '{seat.SeatName}' không hợp lệ hoặc không hoạt động.");

                    var zone = row.Zone;
                    if (zone == null || zone.IsDeleted || zone.Status != CatalogStatus.Active)
                        return Fail($"Khu vực của ghế '{seat.SeatName}' không hợp lệ hoặc không hoạt động.");

                    if (!zone.IsSalable)
                        return Fail($"Khu vực '{zone.ZoneName}' của ghế '{seat.SeatName}' không được phép bán vé.");

                    if (zone.SeatMapId != @event.SeatMapId.Value)
                        return Fail($"Ghế '{seat.SeatName}' không thuộc sơ đồ ghế của sự kiện này.");
                }

                return new GrpcValidationResult(true, "Validation Seat Lock thành công.");
            }
            catch (Exception ex)
            {
                return Fail($"Lỗi hệ thống: {ex.Message}");
            }
        }

        private static GrpcValidationResult Fail(string message) => new(false, message);
    }
}
