using BuildingBlocks.Contracts.Models.Pagination;
using Ordering.Common.Dtos.Reports;

namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IReportService
    {
        Task<(bool IsSuccess, string Message, EventReportDto? Data)> GetEventReportAsync(Guid eventId, Guid userId, bool isAdminOrMod, CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, PaginatedResult<EventOrderListItemDto>? Data)> GetEventOrdersAsync(Guid eventId, Guid userId, bool isAdminOrMod, GetEventOrdersRequest request, CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, List<EventChartDataPointDto>? Data)> GetEventChartDataAsync(Guid eventId, Guid userId, bool isAdminOrMod, string range, CancellationToken cancellationToken = default);
    }
}
