using BuildingBlocks.Domain.DDD;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Domain.Interfaces
{
    public interface IUnitOfWork : IBaseUnitOfWork<IdentityDbContext>
    {
    }
}
