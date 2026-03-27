using BuildingBlocks.Domain.DDD;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Domain.Interfaces.IIdentity_AuthRepositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken, IdentityDbContext>
    {
    }
}
