using Finance.Common.Dtos.Reports;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IServices;

namespace Finance.Infrastructure.Services
{
    public class AdminFinanceReportService : IAdminFinanceReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminFinanceReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminFinanceSummaryDto> GetSummaryAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            List<AcceptedPayoutProjection> payouts = await GetAcceptedPayoutsAsync(dateFrom, dateTo, cancellationToken);

            int pendingPayoutRequestsCount = (await _unitOfWork.PayoutRequestRepository.GetAllAsync<Guid>(
                filter: pr => pr.Status == PayoutRequestStatus.Pending,
                selector: pr => pr.Id,
                cancellation: cancellationToken)).Count();

            decimal currentTotalWalletBalance = (await _unitOfWork.WalletRepository.GetAllAsync<decimal>(
                selector: w => w.Balance,
                cancellation: cancellationToken)).Sum();

            return new AdminFinanceSummaryDto
            {
                GrossRevenue = payouts.Sum(p => p.GrossAmount),
                PlatformFee = payouts.Sum(p => p.FeeAmount),
                NetPaidToOrganizers = payouts.Sum(p => p.NetAmount),
                EventsSettledCount = payouts.Select(p => p.EventId).Distinct().Count(),
                PendingPayoutRequestsCount = pendingPayoutRequestsCount,
                CurrentTotalWalletBalance = currentTotalWalletBalance
            };
        }

        public async Task<List<AdminFinanceTrendPointDto>> GetTrendAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            List<AcceptedPayoutProjection> payouts = await GetAcceptedPayoutsAsync(dateFrom, dateTo, cancellationToken);

            List<AdminFinanceTrendPointDto> trend = payouts
                .GroupBy(p => p.AcceptedAt.Date)
                .Select(g => new AdminFinanceTrendPointDto
                {
                    Date = g.Key,
                    GrossRevenue = g.Sum(p => p.GrossAmount),
                    PlatformFee = g.Sum(p => p.FeeAmount),
                    NetAmount = g.Sum(p => p.NetAmount)
                })
                .OrderBy(p => p.Date)
                .ToList();

            return trend;
        }

        public async Task<List<AdminFinanceByCategoryDto>> GetByCategoryAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            List<AcceptedPayoutProjection> payouts = await GetAcceptedPayoutsAsync(dateFrom, dateTo, cancellationToken);

            Dictionary<Guid, string> categoryNames = (await _unitOfWork.CommissionSettingRepository.GetAllAsync<CommissionSetting>(
                cancellation: cancellationToken))
                .ToDictionary(c => c.CategoryId, c => c.CategoryName);

            List<AdminFinanceByCategoryDto> byCategory = payouts
                .GroupBy(p => p.CategoryId)
                .Select(g => new AdminFinanceByCategoryDto
                {
                    CategoryId = g.Key,
                    CategoryName = categoryNames.TryGetValue(g.Key, out string? name) ? name : "Không xác định",
                    GrossRevenue = g.Sum(p => p.GrossAmount),
                    PlatformFee = g.Sum(p => p.FeeAmount),
                    EventCount = g.Select(p => p.EventId).Distinct().Count()
                })
                .OrderByDescending(c => c.GrossRevenue)
                .ToList();

            return byCategory;
        }

        private async Task<List<AcceptedPayoutProjection>> GetAcceptedPayoutsAsync(DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken)
        {
            IEnumerable<AcceptedPayoutProjection> payouts = await _unitOfWork.EventPayoutRepository.GetAllAsync<AcceptedPayoutProjection>(
                filter: p => p.Status == EventPayoutStatus.Accepted && p.AcceptedAt >= dateFrom && p.AcceptedAt <= dateTo,
                selector: p => new AcceptedPayoutProjection(p.EventId, p.CategoryId, p.GrossAmount, p.FeeAmount, p.NetAmount, p.AcceptedAt!.Value),
                cancellation: cancellationToken);

            return payouts.ToList();
        }

        private sealed record AcceptedPayoutProjection(Guid EventId, Guid CategoryId, decimal GrossAmount, decimal FeeAmount, decimal NetAmount, DateTime AcceptedAt);
    }
}
