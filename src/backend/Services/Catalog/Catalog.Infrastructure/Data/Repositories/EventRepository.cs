using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventRepository : BaseRepository<Event, CatalogDbContext>, IEventRepository
    {
        public EventRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}