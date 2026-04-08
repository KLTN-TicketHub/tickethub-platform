using BuildingBlocks.Infrastructure.Data;
using Identity.Domain.Interfaces;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;

namespace Identity.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
    {
        public UnitOfWork(IdentityDbContext dbContext,
            IRefreshTokenRepository refreshTokenRepository) : base(dbContext)
        {
            RefreshTokenRepository = refreshTokenRepository;
        }

        public IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
