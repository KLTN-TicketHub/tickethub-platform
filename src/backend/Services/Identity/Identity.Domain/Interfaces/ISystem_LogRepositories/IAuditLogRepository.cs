using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Auditing;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Domain.Interfaces.ISystem_LogRepositories
{
    public interface IAuditLogRepository : IBaseRepository<AuditLog, IdentityDbContext>
    {
    }
}
