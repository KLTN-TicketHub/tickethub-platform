using Payment.Common.Dtos.Reports;

namespace Payment.Infrastructure.Interfaces.IServices
{
    public interface IAdminPaymentReportService
    {
        Task<List<AdminPaymentGatewayStatsDto>> GetGatewayStatsAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default);
    }
}
