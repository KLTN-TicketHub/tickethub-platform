using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventClickStatRepository : BaseRepository<EventClickStat, CatalogDbContext>, IEventClickStatRepository
    {
        public EventClickStatRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<(DateOnly StatDate, EventClickType ClickType, long Total)>> GetTrendByEventAsync(
            Guid eventId, DateOnly from, DateOnly to, CancellationToken cancellation = default)
        {
            var rows = await _dbContext.Set<EventClickStat>()
                .Where(s => s.EventId == eventId && s.StatDate >= from && s.StatDate <= to)
                .GroupBy(s => new { s.StatDate, s.ClickType })
                .Select(g => new { g.Key.StatDate, g.Key.ClickType, Total = g.Sum(x => x.ClickCount) })
                .ToListAsync(cancellation);

            return rows.Select(r => (r.StatDate, r.ClickType, r.Total)).ToList();
        }

        public async Task<List<(DateOnly StatDate, EventClickType ClickType, long Total)>> GetTrendByOrganizerAsync(
            Guid organizerId, DateOnly from, DateOnly to, CancellationToken cancellation = default)
        {
            var rows = await _dbContext.Set<EventClickStat>()
                .Where(s => s.Event!.OrganizerId == organizerId && s.StatDate >= from && s.StatDate <= to)
                .GroupBy(s => new { s.StatDate, s.ClickType })
                .Select(g => new { g.Key.StatDate, g.Key.ClickType, Total = g.Sum(x => x.ClickCount) })
                .ToListAsync(cancellation);

            return rows.Select(r => (r.StatDate, r.ClickType, r.Total)).ToList();
        }

        public async Task<List<(Guid EventId, string EventTitle, long ViewCount, long PurchaseIntentCount)>> GetTopEventsByOrganizerAsync(
            Guid organizerId, DateOnly from, DateOnly to, int top, CancellationToken cancellation = default)
        {
            var rows = await _dbContext.Set<EventClickStat>()
                .Where(s => s.Event!.OrganizerId == organizerId && s.StatDate >= from && s.StatDate <= to)
                .GroupBy(s => new { s.EventId, s.Event!.Title })
                .Select(g => new
                {
                    g.Key.EventId,
                    EventTitle = g.Key.Title,
                    ViewCount = g.Sum(x => x.ClickType == EventClickType.ViewDetail ? x.ClickCount : 0),
                    PurchaseIntentCount = g.Sum(x => x.ClickType == EventClickType.PurchaseIntent ? x.ClickCount : 0)
                })
                .OrderByDescending(x => x.ViewCount)
                .Take(top)
                .ToListAsync(cancellation);

            return rows.Select(r => (r.EventId, r.EventTitle, r.ViewCount, r.PurchaseIntentCount)).ToList();
        }

        public async Task<List<(Guid CategoryId, string CategoryName, long ViewCount, long PurchaseIntentCount, int ActiveEventCount)>> GetCategoryTrendAsync(
            DateOnly from, DateOnly to, CancellationToken cancellation = default)
        {
            var rows = await _dbContext.Set<EventClickStat>()
                .Where(s => s.Event!.Status == EventStatus.Published && s.StatDate >= from && s.StatDate <= to)
                .GroupBy(s => new { s.Event!.CategoryId, s.Event!.Category!.CategoryName })
                .Select(g => new
                {
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    ViewCount = g.Sum(x => x.ClickType == EventClickType.ViewDetail ? x.ClickCount : 0),
                    PurchaseIntentCount = g.Sum(x => x.ClickType == EventClickType.PurchaseIntent ? x.ClickCount : 0),
                    ActiveEventCount = g.Select(x => x.EventId).Distinct().Count()
                })
                .ToListAsync(cancellation);

            return rows.Select(r => (r.CategoryId, r.CategoryName, r.ViewCount, r.PurchaseIntentCount, r.ActiveEventCount)).ToList();
        }

        public async Task<long> GetTotalViewsByOrganizerAsync(Guid organizerId, CancellationToken cancellation = default)
        {
            long? total = await _dbContext.Set<EventClickStat>()
                .Where(s => s.Event!.OrganizerId == organizerId && s.ClickType == EventClickType.ViewDetail)
                .SumAsync(s => (long?)s.ClickCount, cancellation);

            return total ?? 0;
        }
    }
}
