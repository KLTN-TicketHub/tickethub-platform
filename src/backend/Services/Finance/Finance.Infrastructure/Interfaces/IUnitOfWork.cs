using BuildingBlocks.Domain.DDD;
using Finance.Infrastructure.Interfaces.IRepositories;

namespace Finance.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IAuditLogRepository AuditLogRepository { get; }
    }
}
