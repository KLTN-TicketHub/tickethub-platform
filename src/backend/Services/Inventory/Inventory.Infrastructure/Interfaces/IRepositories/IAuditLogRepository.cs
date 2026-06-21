using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Inventory.Infrastructure.Data.Contexts;

namespace Inventory.Infrastructure.Interfaces.IRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, InventoryDbContext>
    {
    }
}
