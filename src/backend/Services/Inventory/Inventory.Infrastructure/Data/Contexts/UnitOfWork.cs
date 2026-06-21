using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Interfaces;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<InventoryDbContext>, IUnitOfWork
    {
        public UnitOfWork(
            InventoryDbContext dbContext,
            IAuditLogRepository auditLogRepository) : base(dbContext)
        {
            AuditLogRepository = auditLogRepository;
        }

        public IAuditLogRepository AuditLogRepository { get; }
    }
}
