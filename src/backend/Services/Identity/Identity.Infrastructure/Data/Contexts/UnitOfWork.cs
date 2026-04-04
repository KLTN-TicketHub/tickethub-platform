using BuildingBlocks.Domain.DDD;
using BuildingBlocks.Infrastructure.Data;
using Identity.Domain.Interfaces;

namespace Identity.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<IdentityDbContext>, IUnitOfWork
    {
        public UnitOfWork(IdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
