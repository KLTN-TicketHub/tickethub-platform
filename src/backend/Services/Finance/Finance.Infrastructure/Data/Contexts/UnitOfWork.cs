using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<FinanceDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            FinanceDbContext dbContext,
            IAuditLogRepository auditLogRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
            WalletRepository = walletRepository;
            WalletTransactionRepository = walletTransactionRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IWalletTransactionRepository WalletTransactionRepository { get; }
    }
}
