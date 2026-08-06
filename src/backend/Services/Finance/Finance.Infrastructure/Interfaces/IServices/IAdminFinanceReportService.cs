using Finance.Common.Dtos.Reports;

namespace Finance.Infrastructure.Interfaces.IServices
{
    public interface IAdminFinanceReportService
    {
        Task<AdminFinanceSummaryDto> GetSummaryAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);

        Task<List<AdminFinanceTrendPointDto>> GetTrendAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);

        Task<List<AdminFinanceByCategoryDto>> GetByCategoryAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
    }
}
