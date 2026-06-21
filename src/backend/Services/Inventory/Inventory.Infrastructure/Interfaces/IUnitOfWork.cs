using BuildingBlocks.Domain.DDD;
using Inventory.Infrastructure.Interfaces.IRepositories;

namespace Inventory.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
    }
}
