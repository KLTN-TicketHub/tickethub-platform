using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventCheckInRepository : BaseRepository<EventCheckIn, CatalogDbContext>, IEventCheckInRepository
    {
        public EventCheckInRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
