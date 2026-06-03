using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventApprovalRepository : BaseRepository<EventApproval, DbContext>, IEventApprovalRepository
    {
        public EventApprovalRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}