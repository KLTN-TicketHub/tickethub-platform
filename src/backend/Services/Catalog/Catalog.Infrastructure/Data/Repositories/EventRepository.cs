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

        public async Task<List<(Guid Id, string Title, string Slug, DateTime StartAt, DateTime EndAt, string CoverImageUrl, string CategoryName, decimal MinPrice, string ProvinceCity)>> GetTrendingEventsAsync(
            int count,
            CancellationToken cancellation = default)
        {
            DateTime now = DateTime.UtcNow;

            var trending = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published && e.EndAt >= now)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Slug,
                    e.StartAt,
                    e.EndAt,
                    e.CoverImageUrl,
                    CategoryName = e.Category!.CategoryName,
                    MinPrice = e.ShowTimes.SelectMany(st => st.TicketTypes).Min(tt => tt.Price),
                    ProvinceCity = e.Location.ProvinceCity,
                    TotalClicks = _dbContext.Set<EventClickStat>().Where(cs => cs.EventId == e.Id).Sum(cs => (long?)cs.ClickCount) ?? 0
                })
                .OrderByDescending(x => x.TotalClicks)
                .ThenBy(x => x.StartAt)
                .Take(count)
                .ToListAsync(cancellation);

            return trending
                .Select(x => (x.Id, x.Title, x.Slug, x.StartAt, x.EndAt, x.CoverImageUrl, x.CategoryName, x.MinPrice, x.ProvinceCity))
                .ToList();
        }
    }
}