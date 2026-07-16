using BuildingBlocks.Contracts.Events.Inventory;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Consumers
{
    public class TicketCheckedInConsumer : IConsumer<TicketCheckedInEvent>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketCheckedInConsumer> _logger;

        public TicketCheckedInConsumer(
            IUnitOfWork unitOfWork,
            ILogger<TicketCheckedInConsumer> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TicketCheckedInEvent> context)
        {
            TicketCheckedInEvent message = context.Message;

            _logger.LogInformation(
                "Received TicketCheckedInEvent: IssuedTicketId={IssuedTicketId}, EventId={EventId}, UserId={UserId}, CorrelationId={CorrelationId}",
                message.IssuedTicketId,
                message.EventId,
                message.UserId,
                message.CorrelationId);

            EventCheckIn? existing = await _unitOfWork.EventCheckInRepository.GetOneUntrackedAsync<EventCheckIn>(
                filter: c => c.IssuedTicketId == message.IssuedTicketId,
                cancellation: context.CancellationToken);

            if (existing != null)
            {
                _logger.LogInformation("EventCheckIn đã tồn tại cho IssuedTicketId: {IssuedTicketId}, bỏ qua.", message.IssuedTicketId);
                return;
            }

            EventCheckIn checkIn = new EventCheckIn(message.EventId, message.UserId, message.IssuedTicketId, message.CheckedInAt);

            await _unitOfWork.EventCheckInRepository.CreateAsync(checkIn, context.CancellationToken);

            _logger.LogInformation("Successfully created EventCheckIn for IssuedTicketId: {IssuedTicketId}", message.IssuedTicketId);
        }
    }
}
