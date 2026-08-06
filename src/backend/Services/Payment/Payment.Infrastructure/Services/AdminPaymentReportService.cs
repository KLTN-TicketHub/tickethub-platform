using Payment.Common.Dtos.Reports;
using Payment.Infrastructure.Entities;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IServices;

namespace Payment.Infrastructure.Services
{
    public class AdminPaymentReportService : IAdminPaymentReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminPaymentReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminPaymentGatewayStatsDto>> GetGatewayStatsAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            IEnumerable<TransactionProjection> transactions = await _unitOfWork.PaymentTransactionRepository.GetAllAsync<TransactionProjection>(
                filter: t => t.CreatedAt >= dateFrom && t.CreatedAt <= dateTo,
                selector: t => new TransactionProjection(t.Gateway, t.Status, t.Amount),
                cancellation: cancellationToken);

            List<AdminPaymentGatewayStatsDto> stats = transactions
                .GroupBy(t => t.Gateway)
                .Select(g =>
                {
                    int total = g.Count();
                    int successCount = g.Count(t => t.Status == PaymentStatus.Success);

                    return new AdminPaymentGatewayStatsDto
                    {
                        Gateway = g.Key,
                        TotalTransactions = total,
                        SuccessCount = successCount,
                        FailedCount = g.Count(t => t.Status == PaymentStatus.Failed),
                        PendingCount = g.Count(t => t.Status == PaymentStatus.Pending),
                        SuccessAmount = g.Where(t => t.Status == PaymentStatus.Success).Sum(t => t.Amount),
                        SuccessRate = total > 0 ? Math.Round((double)successCount / total * 100, 2) : 0
                    };
                })
                .OrderByDescending(s => s.SuccessAmount)
                .ToList();

            return stats;
        }

        private sealed record TransactionProjection(string Gateway, PaymentStatus Status, decimal Amount);
    }
}
