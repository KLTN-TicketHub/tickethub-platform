using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class SeatMapRepository : BaseRepository<SeatMap, CatalogDbContext>, ISeatMapRepository
    {
        public SeatMapRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}