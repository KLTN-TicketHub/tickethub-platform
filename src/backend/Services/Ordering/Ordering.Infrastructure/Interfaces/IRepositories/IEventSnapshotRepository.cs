using BuildingBlocks.Domain.DDD;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Interfaces.IRepositories
{
    public interface IEventSnapshotRepository : IBaseRepository<EventSnapshot, OrderingDbContext>
    {
        /// <summary>
        /// Get EventSnapshot with showtimes and ticket types by EventId
        /// </summary>
        Task<EventSnapshot?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get ShowtimeSnapshot with ticket types by ShowtimeId
        /// </summary>
        Task<ShowtimeSnapshot?> GetShowtimeByIdAsync(Guid showtimeId, CancellationToken cancellationToken = default);
    }
}
