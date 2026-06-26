using BuildingBlocks.Domain.DDD;
using Payment.Infrastructure.Interfaces.IRepositories;

namespace Payment.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
    }
}
