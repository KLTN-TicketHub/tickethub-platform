using BuildingBlocks.Contracts.Events.Event;
using Inventory.Infrastructure.Entities;
using Inventory.Infrastructure.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Consumers
{
    public class EventPublishedConsumer : IConsumer<EventPublishedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventPublishedConsumer> _logger;

        public EventPublishedConsumer(
            IUnitOfWork unitOfWork,
            ILogger<EventPublishedConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventPublishedEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received EventPublishedEvent: EventId={EventId}, CorrelationId={CorrelationId}",
                message.EventId,
                message.CorrelationId);

            foreach (var showtime in message.Showtimes)
            {
                foreach (var ticketType in showtime.TicketTypes)
                {
                    // Only process/create inventories for ticket types that DO NOT reserve seats (standing area or non-seatmap events)
                    if (ticketType.IsReservingSeat)
                    {
                        _logger.LogInformation("Skipping seating ticket type: {TicketTypeId} for Showtime: {ShowtimeId}", ticketType.TicketTypeId, showtime.ShowTimeId);
                        continue;
                    }

                    // Check if ShowtimeTicketInventory already exists
                    var existingInventory = await _unitOfWork.ShowtimeTicketInventoryRepository.GetOneAsync<ShowtimeTicketInventory>(
                        filter: x => x.ShowTimeId == showtime.ShowTimeId && x.TicketTypeId == ticketType.TicketTypeId,
                        cancellation: context.CancellationToken
                    );

                    if (existingInventory == null)
                    {
                        var inventory = new ShowtimeTicketInventory
                        {
                            ShowTimeId = showtime.ShowTimeId,
                            TicketTypeId = ticketType.TicketTypeId,
                            Capacity = ticketType.Capacity,
                            SoldQuantity = 0,
                            ReservedQuantity = 0
                        };

                        _unitOfWork.ShowtimeTicketInventoryRepository.AddEntity(inventory);
                        _logger.LogInformation(
                            "Adding new ShowtimeTicketInventory: ShowtimeId={ShowtimeId}, TicketTypeId={TicketTypeId}, Capacity={Capacity}",
                            showtime.ShowTimeId,
                            ticketType.TicketTypeId,
                            ticketType.Capacity);
                    }
                    else
                    {
                        if (existingInventory.Capacity != ticketType.Capacity)
                        {
                            existingInventory.Capacity = ticketType.Capacity;
                            _unitOfWork.ShowtimeTicketInventoryRepository.UpdateEntity(existingInventory);
                            _logger.LogInformation(
                                "Updating ShowtimeTicketInventory Capacity: ShowtimeId={ShowtimeId}, TicketTypeId={TicketTypeId}, NewCapacity={Capacity}",
                                showtime.ShowTimeId,
                                ticketType.TicketTypeId,
                                ticketType.Capacity);
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
            _logger.LogInformation("Successfully processed EventPublishedEvent for EventId={EventId}", message.EventId);
        }
    }
}
