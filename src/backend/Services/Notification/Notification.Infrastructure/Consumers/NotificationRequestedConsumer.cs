using BuildingBlocks.Contracts.Events.Notification;
using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Infrastructure.Interfaces.IServices;

namespace Notification.Infrastructure.Consumers
{
    public class NotificationRequestedConsumer : IConsumer<NotificationRequestedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationRequestedConsumer> _logger;

        public NotificationRequestedConsumer(
            INotificationService notificationService,
            ILogger<NotificationRequestedConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NotificationRequestedEvent> context)
        {
            NotificationRequestedEvent message = context.Message;

            try
            {
                await _notificationService.CreateAsync(message, context.CancellationToken);

                _logger.LogInformation(
                    "Created notification Type={Type} for RecipientUserId={RecipientUserId}, TargetRole={TargetRole}, CorrelationId={CorrelationId}",
                    message.Type, message.RecipientUserId, message.TargetRole, message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create notification Type={Type}, CorrelationId={CorrelationId}",
                    message.Type, message.CorrelationId);
                throw;
            }
        }
    }
}
