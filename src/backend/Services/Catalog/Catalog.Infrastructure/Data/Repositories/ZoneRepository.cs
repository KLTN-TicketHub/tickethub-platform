using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class ZoneRepository : BaseRepository<Zone, CatalogDbContext>, IZoneRepository
    {
        public ZoneRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}