using BuildingBlocks.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces.IRepositories;

namespace Ordering.Infrastructure.Data.Repositories
{
    public class EventSnapshotRepository : BaseRepository<EventSnapshot, OrderingDbContext>, IEventSnapshotRepository
    {
        private readonly OrderingDbContext _context;

        public EventSnapshotRepository(OrderingDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<EventSnapshot?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return await _context.EventSnapshots
                .Include(e => e.Showtimes)
                    .ThenInclude(s => s.TicketTypes)
                .FirstOrDefaultAsync(e => e.EventId == eventId, cancellationToken);
        }

        public async Task<ShowtimeSnapshot?> GetShowtimeByIdAsync(Guid showtimeId, CancellationToken cancellationToken = default)
        {
            return await _context.ShowtimeSnapshots
                .Include(s => s.TicketTypes)
                .Include(s => s.EventSnapshot)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId, cancellationToken);
        }
    }
}
