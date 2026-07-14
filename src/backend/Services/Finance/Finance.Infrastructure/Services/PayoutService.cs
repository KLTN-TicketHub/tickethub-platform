using Finance.Common.Dtos.Payouts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IRepositories;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PayoutService> _logger;

        public PayoutService(IUnitOfWork unitOfWork, ILogger<PayoutService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<EventPendingPayoutDto>> GetEventsPendingPayoutAsync(CancellationToken cancellationToken = default)
        {
            List<PendingPayoutSummary> summaries = (await _unitOfWork.WalletTransactionRepository
                .GetPendingPayoutSummaryAsync(cancellationToken)).ToList();

            List<Guid> categoryIds = summaries.Select(s => s.CategoryId).Distinct().ToList();

            IEnumerable<CommissionSetting> settings = await _unitOfWork.CommissionSettingRepository.GetAllAsync<CommissionSetting>(
                filter: cs => categoryIds.Contains(cs.CategoryId),
                cancellation: cancellationToken);

            Dictionary<Guid, CommissionSetting> settingsByCategory = settings.ToDictionary(s => s.CategoryId);

            return summaries.Select(summary =>
            {
                settingsByCategory.TryGetValue(summary.CategoryId, out CommissionSetting? setting);

                return new EventPendingPayoutDto
                {
                    EventId = summary.EventId,
                    CategoryId = summary.CategoryId,
                    CategoryName = setting?.CategoryName ?? string.Empty,
                    OrganizerId = summary.OrganizerId,
                    GrossAmount = summary.GrossAmount,
                    RecommendedRate = setting?.Rate ?? 0,
                    OrderCount = summary.OrderCount
                };
            }).ToList();
        }

        public async Task<(bool IsSuccess, string Message, EventPayoutResultDto? Data)> ReleaseEventFundsAsync(
            Guid eventId,
            decimal appliedRate,
            Guid reviewerUserId,
            string? reviewerName,
            CancellationToken cancellationToken = default)
        {
            if (appliedRate < 0 || appliedRate > 100)
                return (false, "Phần trăm hoa hồng áp dụng phải nằm trong khoảng từ 0 đến 100.", null);

            List<WalletTransaction> pendingTransactions = (await _unitOfWork.WalletTransactionRepository.GetAllAsync<WalletTransaction>(
                filter: t => t.EventId == eventId
                          && t.Status == WalletTransactionStatus.Pending
                          && t.Type == WalletTransactionType.Revenue
                          && t.ReleaseAt <= DateTime.UtcNow,
                cancellation: cancellationToken)).ToList();

            if (!pendingTransactions.Any())
                return (false, $"Không tìm thấy giao dịch doanh thu nào đang chờ giải ngân cho sự kiện {eventId}.", null);

            Guid walletId = pendingTransactions.First().WalletId;
            Guid categoryId = pendingTransactions.First().CategoryId;
            decimal grossAmount = pendingTransactions.Sum(t => t.Amount);

            Wallet? wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId, cancellationToken);
            if (wallet == null)
                return (false, $"Không tìm thấy ví với ID {walletId}.", null);

            CommissionSetting? setting = await _unitOfWork.CommissionSettingRepository.GetOneUntrackedAsync<CommissionSetting>(
                filter: cs => cs.CategoryId == categoryId,
                cancellation: cancellationToken);

            EventPayout payout = new EventPayout(
                eventId: eventId,
                categoryId: categoryId,
                organizerId: wallet.OrganizerId,
                walletId: wallet.Id,
                grossAmount: grossAmount,
                recommendedRate: setting?.Rate ?? 0,
                appliedRate: appliedRate,
                reviewedByUserId: reviewerUserId,
                reviewedByName: reviewerName);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _unitOfWork.EventPayoutRepository.AddEntity(payout);

                wallet.Credit(payout.NetAmount);
                _unitOfWork.WalletRepository.UpdateEntity(wallet);

                foreach (WalletTransaction tx in pendingTransactions)
                {
                    tx.AssignPayout(payout.Id);
                    _unitOfWork.WalletTransactionRepository.UpdateEntity(tx);
                }

                if (payout.FeeAmount > 0)
                {
                    WalletTransaction feeTransaction = new WalletTransaction(
                        walletId: wallet.Id,
                        orderId: null,
                        eventId: eventId,
                        categoryId: categoryId,
                        amount: payout.FeeAmount,
                        type: WalletTransactionType.Fee,
                        description: $"Phí hoa hồng sàn ({appliedRate}%) cho sự kiện {eventId}",
                        releaseAt: DateTime.UtcNow);
                    feeTransaction.AssignPayout(payout.Id);

                    _unitOfWork.WalletTransactionRepository.AddEntity(feeTransaction);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi xảy ra khi giải ngân cho sự kiện {EventId}", eventId);
                throw;
            }

            EventPayoutResultDto result = new EventPayoutResultDto
            {
                Id = payout.Id,
                EventId = payout.EventId,
                OrganizerId = payout.OrganizerId,
                GrossAmount = payout.GrossAmount,
                AppliedRate = payout.AppliedRate,
                FeeAmount = payout.FeeAmount,
                NetAmount = payout.NetAmount,
                ReviewedAt = payout.ReviewedAt
            };

            return (true, "Đã giải ngân cho sự kiện thành công.", result);
        }
    }
}
