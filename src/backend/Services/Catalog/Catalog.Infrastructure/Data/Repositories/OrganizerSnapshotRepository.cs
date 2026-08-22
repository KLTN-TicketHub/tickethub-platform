using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class OrganizerSnapshotRepository : BaseRepository<OrganizerSnapshot, CatalogDbContext>, IOrganizerSnapshotRepository
    {
        public OrganizerSnapshotRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<(Guid Id, string OrganizerName, string? ImageUrl, int PublishedEventCount)>> GetFeaturedOrganizersAsync(
            int count,
            CancellationToken cancellation = default)
        {
            List<(Guid Id, string OrganizerName, string? ImageUrl, int PublishedEventCount)> organizers = await _dbContext.Set<OrganizerSnapshot>()
                .Where(o => !o.IsDeleted)
                .Select(o => new
                {
                    o.Id,
                    o.OrganizerName,
                    o.ImageUrl,
                    PublishedEventCount = o.Events.Count(e => !e.IsDeleted && e.Status == EventStatus.Published)
                })
                .Where(x => x.PublishedEventCount > 0)
                .Select(x => new ValueTuple<Guid, string, string?, int>(x.Id, x.OrganizerName, x.ImageUrl, x.PublishedEventCount))
                .ToListAsync(cancellation);

            if (organizers.Count == 0)
                return organizers;

            var clicksByOrganizer = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published)
                .Join(_dbContext.Set<EventClickStat>(), e => e.Id, cs => cs.EventId, (e, cs) => new { e.OrganizerId, cs.ClickCount })
                .GroupBy(x => x.OrganizerId)
                .Select(g => new { OrganizerId = g.Key, TotalClicks = g.Sum(x => (long)x.ClickCount) })
                .ToDictionaryAsync(x => x.OrganizerId, x => x.TotalClicks, cancellation);

            var ratingsByOrganizer = await _dbContext.Set<Event>()
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published)
                .Join(_dbContext.Set<EventRating>(), e => e.Id, r => r.EventId, (e, r) => new
                {
                    e.OrganizerId,
                    Score = (r.SoundRating + r.VisualRating + r.OrganizationRating + r.FacilityRating) / 4.0
                })
                .GroupBy(x => x.OrganizerId)
                .Select(g => new { OrganizerId = g.Key, AvgRating = g.Average(x => x.Score) })
                .ToDictionaryAsync(x => x.OrganizerId, x => x.AvgRating, cancellation);

            return organizers
                .OrderByDescending(x => x.PublishedEventCount)
                .ThenByDescending(x => clicksByOrganizer.GetValueOrDefault(x.Id, 0))
                .ThenByDescending(x => ratingsByOrganizer.GetValueOrDefault(x.Id, 0))
                .Take(count)
                .ToList();
        }
    }
}
