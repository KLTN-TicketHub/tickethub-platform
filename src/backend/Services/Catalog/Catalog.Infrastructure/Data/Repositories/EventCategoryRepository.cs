using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventCategoryRepository : BaseRepository<EventCategory, CatalogDbContext>, IEventCategoryRepository
    {
        public EventCategoryRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}