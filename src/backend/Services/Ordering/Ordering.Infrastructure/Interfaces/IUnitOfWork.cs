using BuildingBlocks.Domain.DDD;
using Ordering.Infrastructure.Interfaces.IRepositories;

namespace Ordering.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
    }
}
