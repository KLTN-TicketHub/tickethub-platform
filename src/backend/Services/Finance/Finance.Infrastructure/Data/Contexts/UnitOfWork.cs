using BuildingBlocks.Infrastructure.Data;
using Finance.Infrastructure.Interfaces;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<FinanceDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            FinanceDbContext dbContext,
            IAuditLogRepository auditLogRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
    }
}
