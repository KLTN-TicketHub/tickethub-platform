using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Interfaces.IRepositories;
using Inventory.Infrastructure.Data.Contexts;

namespace Inventory.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, InventoryDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(InventoryDbContext dbContext) : base(dbContext)
        {
        }
    }
}
