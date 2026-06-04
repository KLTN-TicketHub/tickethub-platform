using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class TicketTypeRepository : BaseRepository<TicketType, CatalogDbContext>, ITicketTypeRepository
    {
        public TicketTypeRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}