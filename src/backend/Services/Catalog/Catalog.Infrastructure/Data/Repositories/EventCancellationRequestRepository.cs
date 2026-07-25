using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventCancellationRequestRepository : BaseRepository<EventCancellationRequest, CatalogDbContext>, IEventCancellationRequestRepository
    {
        public EventCancellationRequestRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
