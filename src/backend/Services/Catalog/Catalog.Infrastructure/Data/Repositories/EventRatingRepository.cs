using BuildingBlocks.Infrastructure.Data;
using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Domain.Interfaces.IRepositories;
using Catalog.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data.Repositories
{
    public class EventRatingRepository : BaseRepository<EventRating, CatalogDbContext>, IEventRatingRepository
    {
        public EventRatingRepository(CatalogDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<(Guid CategoryId, double AvgOverallRating, int SampleSize)>> GetCategoryRatingAverageAsync(
            DateOnly from, DateOnly to, CancellationToken cancellation = default)
        {
            DateTime fromUtc = from.ToDateTime(TimeOnly.MinValue);
            DateTime toUtc = to.ToDateTime(TimeOnly.MaxValue);

            var rows = await _dbContext.Set<EventRating>()
                .Where(r => r.Event!.Status == EventStatus.Published && r.CreatedAt >= fromUtc && r.CreatedAt <= toUtc)
                .GroupBy(r => r.Event!.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    AvgOverallRating = g.Average(x => x.OverallRating),
                    SampleSize = g.Count()
                })
                .ToListAsync(cancellation);

            return rows.Select(r => (r.CategoryId, r.AvgOverallRating, r.SampleSize)).ToList();
        }

        public async Task<List<(Guid EventId, double SoundAvg, double VisualAvg, double OrganizationAvg, double FacilityAvg, double ServiceAvg, double PerformanceAvg, double OverallAvg, int SampleSize)>> GetRatingSummaryByEventIdsAsync(
            List<Guid> eventIds, CancellationToken cancellation = default)
        {
            var rows = await _dbContext.Set<EventRating>()
                .Where(r => eventIds.Contains(r.EventId))
                .GroupBy(r => r.EventId)
                .Select(g => new
                {
                    EventId = g.Key,
                    SoundAvg = g.Average(x => (double)x.SoundRating),
                    VisualAvg = g.Average(x => (double)x.VisualRating),
                    OrganizationAvg = g.Average(x => (double)x.OrganizationRating),
                    FacilityAvg = g.Average(x => (double)x.FacilityRating),
                    ServiceAvg = g.Average(x => (double)x.ServiceRating),
                    PerformanceAvg = g.Average(x => (double)x.PerformanceRating),
                    OverallAvg = g.Average(x => x.OverallRating),
                    SampleSize = g.Count()
                })
                .ToListAsync(cancellation);

            return rows.Select(r => (r.EventId, r.SoundAvg, r.VisualAvg, r.OrganizationAvg, r.FacilityAvg, r.ServiceAvg, r.PerformanceAvg, r.OverallAvg, r.SampleSize)).ToList();
        }

        public async Task<List<string>> GetRecentCommentsByEventIdAsync(
            Guid eventId, int take, CancellationToken cancellation = default)
        {
            return await _dbContext.Set<EventRating>()
                .Where(r => r.EventId == eventId && r.Comment != null && r.Comment != "")
                .OrderByDescending(r => r.CreatedAt)
                .Take(take)
                .Select(r => r.Comment!)
                .ToListAsync(cancellation);
        }
    }
}
