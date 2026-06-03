using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class ZoneRepository : BaseRepository<Zone, DbContext>, IZoneRepository
    {
        public ZoneRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}