using BuildingBlocks.Contracts.Models.Pagination;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Common.Dtos.Reports;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces.IServices;

namespace Ordering.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly OrderingDbContext _dbContext;
        private readonly ILogger<ReportService> _logger;

        public ReportService(OrderingDbContext dbContext, ILogger<ReportService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<EventReportDto> GetEventReportAsync(
            Guid eventId,
            Guid userId,
            bool isAdminOrMod,
            CancellationToken cancellationToken = default)
        {
            try
            {
                EventSnapshot? eventSnapshot = await _dbContext.EventSnapshots
                    .Include(e => e.Showtimes)
                        .ThenInclude(s => s.TicketTypes)
                    .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

                if (eventSnapshot == null)
                {
                    throw new NotFoundException("Không tìm thấy thông tin sự kiện hoặc sự kiện chưa được phê duyệt.");
                }

                if (!isAdminOrMod && eventSnapshot.OrganizerId != userId)
                {
                    throw new ForbiddenAccessException("Bạn không có quyền xem báo cáo của sự kiện này.");
                }

                List<Order> paidOrders = await _dbContext.Orders
                    .Where(o => o.EventId == eventId && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed))
                    .Include(o => o.OrderItems)
                    .ToListAsync(cancellationToken);

                decimal totalRevenue = paidOrders.Sum(o => o.TotalPrice);
                int totalTicketsSold = paidOrders.SelectMany(o => o.OrderItems).Sum(item => item.Quantity);
                int totalCapacity = eventSnapshot.Showtimes.SelectMany(s => s.TicketTypes).Sum(tt => tt.Capacity);
                double fillRate = totalCapacity > 0 ? Math.Round((double)totalTicketsSold / totalCapacity * 100, 2) : 0.0;

                var showtimesReport = eventSnapshot.Showtimes.Select(s =>
                {
                    var showtimeOrders = paidOrders.Where(o => o.ShowTimeId == s.ShowtimeId).ToList();
                    decimal showtimeRevenue = showtimeOrders.Sum(o => o.TotalPrice);
                    int showtimeTicketsSold = showtimeOrders.SelectMany(o => o.OrderItems).Sum(item => item.Quantity);
                    int showtimeCapacity = s.TicketTypes.Sum(tt => tt.Capacity);
                    double showtimeFillRate = showtimeCapacity > 0 ? Math.Round((double)showtimeTicketsSold / showtimeCapacity * 100, 2) : 0.0;

                    return new ShowtimeReportDto
                    {
                        ShowtimeId = s.ShowtimeId,
                        StartAt = s.StartAt,
                        EndAt = s.EndAt,
                        Revenue = showtimeRevenue,
                        TicketsSold = showtimeTicketsSold,
                        Capacity = showtimeCapacity,
                        FillRate = showtimeFillRate
                    };
                }).ToList();

                var reportDto = new EventReportDto
                {
                    EventId = eventSnapshot.EventId,
                    EventTitle = eventSnapshot.EventTitle,
                    EventImage = eventSnapshot.EventImage,
                    TotalRevenue = totalRevenue,
                    TotalTicketsSold = totalTicketsSold,
                    TotalCapacity = totalCapacity,
                    FillRate = fillRate,
                    Showtimes = showtimesReport
                };

                return reportDto;
            }
            catch (Exception ex) when (ex is not BaseCustomException)
            {
                _logger.LogError(ex, "[ReportService] Error retrieving report for EventId={EventId}", eventId);
                throw new BusinessRuleException($"Lỗi xử lý hệ thống: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<EventOrderListItemDto>> GetEventOrdersAsync(
            Guid eventId,
            Guid userId,
            bool isAdminOrMod,
            GetEventOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                EventSnapshot? eventSnapshot = await _dbContext.EventSnapshots
                    .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

                if (eventSnapshot == null)
                {
                    throw new NotFoundException("Không tìm thấy thông tin sự kiện hoặc sự kiện chưa được phê duyệt.");
                }

                if (!isAdminOrMod && eventSnapshot.OrganizerId != userId)
                {
                    throw new ForbiddenAccessException("Bạn không có quyền xem danh sách đơn hàng của sự kiện này.");
                }

                var query = _dbContext.Orders
                    .Where(o => o.EventId == eventId)
                    .Include(o => o.OrderItems)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Search))
                {
                    string search = request.Search.Trim().ToLower();
                    query = query.Where(o => o.CustomerName.ToLower().Contains(search) ||
                                             o.CustomerEmail.ToLower().Contains(search) ||
                                             o.CustomerPhone.Contains(search) ||
                                             o.Id.ToString().Contains(search));
                }

                int totalCount = await query.CountAsync(cancellationToken);

                var orders = await query
                    .OrderByDescending(o => o.CreatedAt)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var dtos = orders.Select(o => new EventOrderListItemDto
                {
                    OrderId = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerEmail = o.CustomerEmail,
                    CustomerPhone = o.CustomerPhone,
                    ShowtimeId = o.ShowTimeId,
                    ShowtimeStartAt = o.ShowtimeStartAt,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(item => new EventOrderItemDto
                    {
                        SeatId = item.SeatId,
                        SeatName = item.SeatName,
                        RowName = item.RowName,
                        TicketTypeId = item.TicketTypeId,
                        TicketTypeName = item.TicketTypeName,
                        Price = item.Price,
                        Quantity = item.Quantity
                    }).ToList()
                }).ToList();

                var result = new PaginatedResult<EventOrderListItemDto>(dtos, totalCount, request.PageNumber, request.PageSize);
                return result;
            }
            catch (Exception ex) when (ex is not BaseCustomException)
            {
                _logger.LogError(ex, "[ReportService] Error retrieving order list for EventId={EventId}", eventId);
                throw new BusinessRuleException($"Lỗi xử lý hệ thống: {ex.Message}");
            }
        }

        public async Task<List<EventChartDataPointDto>> GetEventChartDataAsync(
            Guid eventId,
            Guid userId,
            bool isAdminOrMod,
            string range,
            CancellationToken cancellationToken = default)
        {
            try
            {
                EventSnapshot? eventSnapshot = await _dbContext.EventSnapshots
                    .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);

                if (eventSnapshot == null)
                {
                    throw new NotFoundException("Không tìm thấy thông tin sự kiện hoặc sự kiện chưa được phê duyệt.");
                }

                if (!isAdminOrMod && eventSnapshot.OrganizerId != userId)
                {
                    throw new ForbiddenAccessException("Bạn không có quyền xem dữ liệu thống kê của sự kiện này.");
                }

                var chartPoints = new List<EventChartDataPointDto>();

                if (range?.ToLower() == "24h")
                {
                    var startTime = DateTime.UtcNow.AddHours(-24);
                    var orders = await _dbContext.Orders
                        .Where(o => o.EventId == eventId &&
                                    (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed) &&
                                    o.CreatedAt >= startTime)
                        .Include(o => o.OrderItems)
                        .ToListAsync(cancellationToken);

                    var current = DateTime.UtcNow.AddHours(-24);
                    current = new DateTime(current.Year, current.Month, current.Day, current.Hour, 0, 0, DateTimeKind.Utc);
                    var nowAligned = DateTime.UtcNow;
                    nowAligned = new DateTime(nowAligned.Year, nowAligned.Month, nowAligned.Day, nowAligned.Hour, 0, 0, DateTimeKind.Utc);

                    while (current <= nowAligned)
                    {
                        var nextHour = current.AddHours(1);
                        var hourlyOrders = orders.Where(o => o.CreatedAt >= current && o.CreatedAt < nextHour).ToList();

                        chartPoints.Add(new EventChartDataPointDto
                        {
                            TotalAmount = hourlyOrders.Sum(o => o.TotalPrice),
                            TicketSold = hourlyOrders.SelectMany(o => o.OrderItems).Sum(i => i.Quantity),
                            Legend = current.ToString("HH:00 dd MMM", System.Globalization.CultureInfo.InvariantCulture)
                        });
                        current = nextHour;
                    }
                }
                else
                {
                    var startTime = DateTime.UtcNow.Date.AddDays(-29);
                    var orders = await _dbContext.Orders
                        .Where(o => o.EventId == eventId &&
                                    (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed) &&
                                    o.CreatedAt >= startTime)
                        .Include(o => o.OrderItems)
                        .ToListAsync(cancellationToken);

                    var current = DateTime.UtcNow.Date.AddDays(-29);
                    var today = DateTime.UtcNow.Date;

                    while (current <= today)
                    {
                        var nextDay = current.AddDays(1);
                        var dailyOrders = orders.Where(o => o.CreatedAt >= current && o.CreatedAt < nextDay).ToList();

                        chartPoints.Add(new EventChartDataPointDto
                        {
                            TotalAmount = dailyOrders.Sum(o => o.TotalPrice),
                            TicketSold = dailyOrders.SelectMany(o => o.OrderItems).Sum(i => i.Quantity),
                            Legend = current.ToString("dd MMM yy", System.Globalization.CultureInfo.InvariantCulture)
                        });
                        current = nextDay;
                    }
                }

                return chartPoints;
            }
            catch (Exception ex) when (ex is not BaseCustomException)
            {
                _logger.LogError(ex, "[ReportService] Error retrieving chart statistics for EventId={EventId}", eventId);
                throw new BusinessRuleException($"Lỗi xử lý hệ thống: {ex.Message}");
            }
        }

        public async Task<OrganizerOrderSummaryDto> GetOrganizerSummaryAsync(
            Guid organizerId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                List<Guid> eventIds = await _dbContext.EventSnapshots
                    .Where(e => e.OrganizerId == organizerId)
                    .Select(e => e.EventId)
                    .ToListAsync(cancellationToken);

                List<Order> paidOrders = await _dbContext.Orders
                    .Where(o => eventIds.Contains(o.EventId) && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed))
                    .Include(o => o.OrderItems)
                    .ToListAsync(cancellationToken);

                return new OrganizerOrderSummaryDto
                {
                    TotalRevenue = paidOrders.Sum(o => o.TotalPrice),
                    TotalTicketsSold = paidOrders.SelectMany(o => o.OrderItems).Sum(item => item.Quantity)
                };
            }
            catch (Exception ex) when (ex is not BaseCustomException)
            {
                _logger.LogError(ex, "[ReportService] Error retrieving organizer summary for OrganizerId={OrganizerId}", organizerId);
                throw new BusinessRuleException($"Lỗi xử lý hệ thống: {ex.Message}");
            }
        }
    }
}
