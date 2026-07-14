using BuildingBlocks.Contracts.Events.Event;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Data.Contexts;
using Ordering.Infrastructure.Entities;

namespace Ordering.Infrastructure.Consumers
{
    public class EventPublishedConsumer : IConsumer<EventPublishedEvent>
    {
        private readonly OrderingDbContext _dbContext;
        private readonly ILogger<EventPublishedConsumer> _logger;

        public EventPublishedConsumer(OrderingDbContext dbContext, ILogger<EventPublishedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventPublishedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "[EventPublishedConsumer] Received EventPublishedEvent for EventId={EventId}",
                message.EventId);

            try
            {
                var existing = await _dbContext.EventSnapshots
                    .Include(e => e.Showtimes)
                        .ThenInclude(s => s.TicketTypes)
                    .FirstOrDefaultAsync(e => e.EventId == message.EventId, context.CancellationToken);

                if (existing != null)
                {
                    _dbContext.EventSnapshots.Remove(existing);
                    await _dbContext.SaveChangesAsync(context.CancellationToken);
                    _logger.LogInformation(
                        "[EventPublishedConsumer] Removed existing snapshot for EventId={EventId} before re-creating.",
                        message.EventId);
                }

                var snapshot = new EventSnapshot(
                    eventId: message.EventId,
                    eventTitle: message.EventTitle,
                    eventImage: message.EventImage,
                    categoryId: message.CategoryId,
                    categoryName: message.CategoryName,
                    organizerId: message.OrganizerId,
                    organizerName: message.OrganizerName,
                    saleOpenAt: message.SaleOpenAt,
                    saleCloseAt: message.SaleCloseAt);

                foreach (var st in message.Showtimes)
                {
                    var showtimeSnapshot = new ShowtimeSnapshot(
                        eventSnapshotId: snapshot.Id,
                        showtimeId: st.ShowTimeId,
                        startAt: st.StartAt,
                        endAt: st.EndAt);

                    foreach (var tt in st.TicketTypes)
                    {
                        var ticketTypeSnapshot = new TicketTypeSnapshot(
                            showtimeSnapshotId: showtimeSnapshot.Id,
                            ticketTypeId: tt.TicketTypeId,
                            ticketTypeName: tt.TicketTypeName,
                            price: tt.Price,
                            capacity: tt.Capacity,
                            isReservingSeat: tt.IsReservingSeat);

                        showtimeSnapshot.AddTicketType(ticketTypeSnapshot);
                    }

                    snapshot.AddShowtime(showtimeSnapshot);
                }

                _dbContext.EventSnapshots.Add(snapshot);
                await _dbContext.SaveChangesAsync(context.CancellationToken);

                _logger.LogInformation(
                    "[EventPublishedConsumer] Successfully persisted EventSnapshot for EventId={EventId} with {ShowtimeCount} showtime(s).",
                    message.EventId, message.Showtimes.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[EventPublishedConsumer] Failed to persist EventSnapshot for EventId={EventId}",
                    message.EventId);
                throw;
            }
        }
    }
}
