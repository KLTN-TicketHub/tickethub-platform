using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace Finance.Infrastructure.Services
{
    public class ReleaseFundsJobService : IReleaseFundsJobService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReleaseFundsJobService> _logger;

        public ReleaseFundsJobService(IUnitOfWork unitOfWork, ILogger<ReleaseFundsJobService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessReleaseFundsAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Bắt đầu thực thi Job giải ngân cho các sự kiện đã kết thúc...");

                IEnumerable<WalletTransaction> pendingTransactions = await _unitOfWork.WalletTransactionRepository.GetAllAsync<WalletTransaction>(
                    filter: t => t.Status == WalletTransactionStatus.Pending &&
                                 t.Type == WalletTransactionType.Revenue &&
                                 t.ReleaseAt <= DateTime.UtcNow,
                    cancellation: cancellationToken
                );

                if (!pendingTransactions.Any())
                {
                    _logger.LogInformation("Không có giao dịch nào cần giải ngân.");
                    return;
                }

                int successCount = await ReleaseFundsForWalletsAsync(pendingTransactions, cancellationToken);

                _logger.LogInformation("Đã giải ngân thành công {Count} giao dịch.", successCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình chạy Job giải ngân.");
                throw;
            }
        }

        private async Task<int> ReleaseFundsForWalletsAsync(
            IEnumerable<WalletTransaction> pendingTransactions,
            CancellationToken cancellationToken)
        {
            int successCount = 0;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                IEnumerable<IGrouping<Guid, WalletTransaction>> transactionsByWallet =
                    pendingTransactions.GroupBy(t => t.WalletId);

                foreach (IGrouping<Guid, WalletTransaction> group in transactionsByWallet)
                {
                    Wallet? wallet = await _unitOfWork.WalletRepository.GetByIdAsync(group.Key, cancellationToken);
                    if (wallet == null)
                    {
                        _logger.LogWarning("Không tìm thấy ví {WalletId} cho các giao dịch chờ giải ngân.", group.Key);
                        continue;
                    }

                    foreach (WalletTransaction tx in group)
                    {
                        wallet.Credit(tx.Amount);
                        tx.MarkAsSuccess();

                        _unitOfWork.WalletTransactionRepository.UpdateEntity(tx);
                        successCount++;
                    }

                    _unitOfWork.WalletRepository.UpdateEntity(wallet);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            return successCount;
        }
    }
}
