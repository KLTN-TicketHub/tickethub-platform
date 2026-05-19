using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Interfaces;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class UnitOfWork : BaseUnitOfWork<CatalogDbContext>, IUnitOfWork
    {
        public UnitOfWork(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
