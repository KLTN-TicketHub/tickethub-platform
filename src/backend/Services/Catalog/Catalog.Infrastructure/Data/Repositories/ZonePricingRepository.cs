using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class ZonePricingRepository : BaseRepository<ZonePricing, CatalogDbContext>, IZonePricingRepository
    {
        public ZonePricingRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}