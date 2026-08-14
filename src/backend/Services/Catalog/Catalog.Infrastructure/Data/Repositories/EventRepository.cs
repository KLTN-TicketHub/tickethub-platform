using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventRepository : BaseRepository<Event, CatalogDbContext>, IEventRepository
    {
        public EventRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Dictionary<EventStatus, int>> GetCountByStatusAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellation = default)
        {
            var counts = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.CreatedAt >= from && e.CreatedAt <= to)
                .GroupBy(e => e.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellation);

            return counts.ToDictionary(x => x.Status, x => x.Count);
        }

        public async Task<List<(Guid CategoryId, string CategoryName, int EventCount)>> GetCountByCategoryAsync(
            DateTime from,
            DateTime to,
            CancellationToken cancellation = default)
        {
            var counts = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.CreatedAt >= from && e.CreatedAt <= to)
                .GroupBy(e => new { e.CategoryId, CategoryName = e.Category!.CategoryName })
                .Select(g => new { g.Key.CategoryId, g.Key.CategoryName, EventCount = g.Count() })
                .OrderByDescending(x => x.EventCount)
                .ToListAsync(cancellation);

            return counts.Select(x => (x.CategoryId, x.CategoryName, x.EventCount)).ToList();
        }
    }
}