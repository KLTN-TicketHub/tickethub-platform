using BuildingBlocks.Infrastructure.Data;
using Identity.Domain.Interfaces;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;
using Identity.Domain.Interfaces.ISystem_LogRepositories;

namespace Identity.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
    {
        public UnitOfWork(IdentityDbContext dbContext,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository) : base(dbContext)
        {
            RefreshTokenRepository = refreshTokenRepository;
            UserRepository = userRepository;
            AuditLogRepository = auditLogRepository;
        }

        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IUserRepository UserRepository { get; }
        public IAuditLogRepository AuditLogRepository { get; }
    }
}
