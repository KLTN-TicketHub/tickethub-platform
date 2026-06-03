using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class TicketTypeRepository : BaseRepository<TicketType, DbContext>, ITicketTypeRepository
    {
        public TicketTypeRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}