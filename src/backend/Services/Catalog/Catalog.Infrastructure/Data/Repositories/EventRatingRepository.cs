using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventRatingRepository : BaseRepository<EventRating, CatalogDbContext>, IEventRatingRepository
    {
        public EventRatingRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
