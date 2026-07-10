using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
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
                    include: q => q.Include(t => t.Wallet),
                    cancellation: cancellationToken
                );

                if (!pendingTransactions.Any())
                {
                    _logger.LogInformation("Không có giao dịch nào cần giải ngân.");
                    return;
                }

                int successCount = 0;
                foreach (var tx in pendingTransactions)
                {
                    if (tx.Wallet == null)
                    {
                        _logger.LogWarning("Không tìm thấy ví cho giao dịch {TxId}", tx.Id);
                        continue;
                    }

                    tx.Wallet.Credit(tx.Amount);
                    tx.MarkAsSuccess();

                    await _unitOfWork.WalletRepository.UpdateAsync(tx.Wallet);
                    await _unitOfWork.WalletTransactionRepository.UpdateAsync(tx);

                    successCount++;
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Đã giải ngân thành công {Count} giao dịch.", successCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình chạy Job giải ngân.");
                throw;
            }
        }
    }
}
