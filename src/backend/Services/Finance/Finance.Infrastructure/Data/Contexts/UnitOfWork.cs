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
            IWalletTransactionRepository walletTransactionRepository,
            ICommissionSettingRepository commissionSettingRepository,
            IEventPayoutRepository eventPayoutRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
            WalletRepository = walletRepository;
            WalletTransactionRepository = walletTransactionRepository;
            CommissionSettingRepository = commissionSettingRepository;
            EventPayoutRepository = eventPayoutRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public IWalletTransactionRepository WalletTransactionRepository { get; }
        public ICommissionSettingRepository CommissionSettingRepository { get; }
        public IEventPayoutRepository EventPayoutRepository { get; }
    }
}
