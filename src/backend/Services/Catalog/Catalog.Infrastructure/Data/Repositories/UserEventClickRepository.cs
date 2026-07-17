using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class UserEventClickRepository : BaseRepository<UserEventClick, CatalogDbContext>, IUserEventClickRepository
    {
        public UserEventClickRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }
    }
}
