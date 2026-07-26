using BuildingBlocks.Contracts.Events.Event;
using BuildingBlocks.Contracts.Events.Order;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure.Entities;
using Ordering.Infrastructure.Interfaces;

namespace Ordering.Infrastructure.Consumers
{
    public class EventCancelledConsumer : IConsumer<EventCancelledEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EventCancelledConsumer> _logger;

        public EventCancelledConsumer(IUnitOfWork unitOfWork, ILogger<EventCancelledConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventCancelledEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing EventCancelledEvent for EventId={EventId}, finding orders that need refunds", message.EventId);

            try
            {
                IEnumerable<Order> orders = await _unitOfWork.OrderRepository.GetAllAsync<Order>(
                    filter: o => o.EventId == message.EventId && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Completed),
                    cancellation: context.CancellationToken);

                foreach (Order order in orders)
                {
                    await context.Publish(new OrderRefundRequestedEvent
                    {
                        OrderId = order.Id,
                        EventId = message.EventId
                    });
                }

                _logger.LogInformation("Initiated refunds for {Count} order(s) of EventId={EventId}", orders.Count(), message.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing EventCancelledEvent for EventId={EventId}", message.EventId);
                throw;
            }
        }
    }
}
