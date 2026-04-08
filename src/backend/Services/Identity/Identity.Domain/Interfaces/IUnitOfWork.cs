using BuildingBlocks.Domain.DDD;
using Identity.Domain.Interfaces.IIdentity_AuthRepositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork<IdentityDbContext>
    {
        IRefreshTokenRepository RefreshTokenRepository { get; }
    }
}
