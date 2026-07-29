using BuildingBlocks.Domain.DDD;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;
using Identity.Domain.Interfaces.ISystem_LogRepositories;

namespace Identity.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IUserRepository UserRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }
    }
}
