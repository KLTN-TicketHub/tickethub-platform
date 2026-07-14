using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;
using Finance.Infrastructure.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    public class WalletTransactionRepository : BaseRepository<WalletTransaction, FinanceDbContext>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(FinanceDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<PendingPayoutSummary?> GetEventPendingSummaryAsync(Guid eventId, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<WalletTransaction>()
                .Where(t => t.EventId == eventId
                         && t.Status == WalletTransactionStatus.Pending
                         && t.Type == WalletTransactionType.Revenue
                         && t.EventPayoutId == null
                         && t.ReleaseAt <= DateTime.UtcNow)
                .GroupBy(t => new { t.EventId, t.EventTitle, t.CategoryId, t.WalletId, t.Wallet.OrganizerId })
                .Select(g => new PendingPayoutSummary
                {
                    EventId = g.Key.EventId,
                    EventTitle = g.Key.EventTitle,
                    CategoryId = g.Key.CategoryId,
                    WalletId = g.Key.WalletId,
                    OrganizerId = g.Key.OrganizerId,
                    GrossAmount = g.Sum(t => t.Amount),
                    OrderCount = g.Count()
                })
                .FirstOrDefaultAsync(cancellation);
        }

        public async Task<bool> HasUnreleasedRevenueAsync(Guid eventId, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<WalletTransaction>()
                .AnyAsync(t => t.EventId == eventId
                            && t.Status == WalletTransactionStatus.Pending
                            && t.Type == WalletTransactionType.Revenue
                            && t.ReleaseAt > DateTime.UtcNow,
                    cancellation);
        }
    }
}
