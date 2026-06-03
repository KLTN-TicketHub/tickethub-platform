using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class VenueRepository : BaseRepository<Venue, DbContext>, IVenueRepository
    {
        public VenueRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
