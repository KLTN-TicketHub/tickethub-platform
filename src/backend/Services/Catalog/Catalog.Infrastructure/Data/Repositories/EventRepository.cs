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

            var clicksByEvent = _dbContext.Set<EventClickStat>()
                .GroupBy(cs => cs.EventId)
                .Select(g => new { EventId = g.Key, TotalClicks = g.Sum(cs => (long)cs.ClickCount) });

            var trending = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published && e.EndAt >= now)
                .GroupJoin(clicksByEvent, e => e.Id, c => c.EventId, (e, clicks) => new { Event = e, Clicks = clicks })
                .SelectMany(x => x.Clicks.DefaultIfEmpty(), (x, c) => new { x.Event, TotalClicks = c != null ? c.TotalClicks : 0 })
                .OrderByDescending(x => x.TotalClicks)
                .ThenBy(x => x.Event.StartAt)
                .Take(count)
                .Select(x => new
                {
                    x.Event.Id,
                    x.Event.Title,
                    x.Event.Slug,
                    x.Event.StartAt,
                    x.Event.EndAt,
                    x.Event.CoverImageUrl,
                    CategoryName = x.Event.Category!.CategoryName,
                    MinPrice = x.Event.ShowTimes.SelectMany(st => st.TicketTypes).Min(tt => tt.Price),
                    ProvinceCity = x.Event.Location.ProvinceCity
                })
                .ToListAsync(cancellation);

            return trending
                .Select(x => (x.Id, x.Title, x.Slug, x.StartAt, x.EndAt, x.CoverImageUrl, x.CategoryName, x.MinPrice, x.ProvinceCity))
                .ToList();
        }
    }
}