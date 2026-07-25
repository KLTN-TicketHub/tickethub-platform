using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Domain.Interfaces.IRepositories
{
    public interface IEventRatingRepository : IBaseRepository<EventRating, DbContext>
    {
        Task<List<(Guid CategoryId, double AvgOverallRating, int SampleSize)>> GetCategoryRatingAverageAsync(
            DateOnly from, DateOnly to, CancellationToken cancellation = default);

        Task<List<(Guid EventId, double SoundAvg, double VisualAvg, double OrganizationAvg, double FacilityAvg, double ServiceAvg, double PerformanceAvg, double OverallAvg, int SampleSize)>> GetRatingSummaryByEventIdsAsync(
            List<Guid> eventIds, CancellationToken cancellation = default);

        Task<List<string>> GetRecentCommentsByEventIdAsync(
            Guid eventId, int take, CancellationToken cancellation = default);
    }
}
