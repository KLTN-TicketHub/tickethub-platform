using BuildingBlocks.Domain.DDD;
using Finance.Infrastructure.Data.Contexts;
using Finance.Infrastructure.Entities;

namespace Finance.Infrastructure.Interfaces.IRepositories
{
    public interface IWalletTransactionRepository : IBaseRepository<WalletTransaction, FinanceDbContext>
    {
        Task<IEnumerable<PendingPayoutSummary>> GetPendingPayoutSummaryAsync(CancellationToken cancellation = default);
    }

    public class PendingPayoutSummary
    {
        public Guid EventId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid WalletId { get; set; }
        public Guid OrganizerId { get; set; }
        public decimal GrossAmount { get; set; }
        public int OrderCount { get; set; }
    }
}
