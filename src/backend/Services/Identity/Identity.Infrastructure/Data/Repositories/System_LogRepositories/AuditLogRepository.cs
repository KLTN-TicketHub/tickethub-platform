using BuildingBlocks.Infrastructure.Auditing;
using BuildingBlocks.Infrastructure.Data;
using Identity.Domain.Interfaces.ISystem_LogRepositories;
using Identity.Infrastructure.Data.Contexts;

namespace Identity.Infrastructure.Data.Repositories.System_LogRepositories
{
    public class AuditLogRepository : BaseRepository<AuditLog, IdentityDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
