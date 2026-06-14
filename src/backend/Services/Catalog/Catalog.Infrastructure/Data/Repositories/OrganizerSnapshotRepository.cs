using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class OrganizerSnapshotRepository : BaseRepository<OrganizerSnapshot, CatalogDbContext>, IOrganizerSnapshotRepository
    {
        public OrganizerSnapshotRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
