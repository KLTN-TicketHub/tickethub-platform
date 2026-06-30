using BuildingBlocks.Infrastructure.Data;
using Payment.Infrastructure.Interfaces;
using Payment.Infrastructure.Interfaces.IRepositories;

namespace Payment.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<PaymentDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            PaymentDbContext dbContext,
            IAuditLogRepository auditLogRepository,
            IPaymentTransactionRepository paymentTransactionRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
            PaymentTransactionRepository = paymentTransactionRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
        public IPaymentTransactionRepository PaymentTransactionRepository { get; }
    }
}
