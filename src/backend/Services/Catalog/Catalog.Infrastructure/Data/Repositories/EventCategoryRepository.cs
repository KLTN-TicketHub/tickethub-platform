using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventCategoryRepository : BaseRepository<EventCategory, DbContext>, IEventCategoryRepository
    {
        public EventCategoryRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}