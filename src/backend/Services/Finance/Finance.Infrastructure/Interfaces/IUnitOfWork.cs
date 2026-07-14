using BuildingBlocks.Domain.DDD;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
        IWalletRepository WalletRepository { get; }
        IWalletTransactionRepository WalletTransactionRepository { get; }
        ICommissionSettingRepository CommissionSettingRepository { get; }
        IEventPayoutRepository EventPayoutRepository { get; }
    }
}
