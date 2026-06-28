using BuildingBlocks.Infrastructure.Data;
using Ordering.Infrastructure.Interfaces;
using Ordering.Infrastructure.Interfaces.IRepositories;

namespace Ordering.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<OrderingDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            OrderingDbContext dbContext,
            IAuditLogRepository auditLogRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
    }
}
