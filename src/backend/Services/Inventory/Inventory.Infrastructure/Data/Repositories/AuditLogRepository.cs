using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Inventory.Infrastructure.Data.Contexts;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Data.Repositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, InventoryDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(InventoryDbContext dbContext) : base(dbContext)
        {
        }
    }
}
