using BuildingBlocks.Contracts.Models.Pagination;
using Ordering.Common.Dtos.Reports;

namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IReportService
    {
        Task<EventReportDto> GetEventReportAsync(Guid eventId, Guid userId, bool isAdminOrMod, CancellationToken cancellationToken = default);

        Task<PaginatedResult<EventOrderListItemDto>> GetEventOrdersAsync(Guid eventId, Guid userId, bool isAdminOrMod, GetEventOrdersRequest request, CancellationToken cancellationToken = default);

        Task<List<EventChartDataPointDto>> GetEventChartDataAsync(Guid eventId, Guid userId, bool isAdminOrMod, string range, CancellationToken cancellationToken = default);
    }
}
