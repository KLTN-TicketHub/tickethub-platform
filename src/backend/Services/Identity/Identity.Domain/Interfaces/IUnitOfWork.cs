using BuildingBlocks.Domain.DDD;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;

namespace Identity.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork
    {
        IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
