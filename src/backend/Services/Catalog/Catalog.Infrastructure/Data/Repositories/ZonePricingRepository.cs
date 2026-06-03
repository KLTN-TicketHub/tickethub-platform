using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class ZonePricingRepository : BaseRepository<ZonePricing, DbContext>, IZonePricingRepository
    {
        public ZonePricingRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}