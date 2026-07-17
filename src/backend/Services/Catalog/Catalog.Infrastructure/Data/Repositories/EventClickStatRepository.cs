using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventClickStatRepository : BaseRepository<EventClickStat, CatalogDbContext>, IEventClickStatRepository
    {
        public EventClickStatRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
