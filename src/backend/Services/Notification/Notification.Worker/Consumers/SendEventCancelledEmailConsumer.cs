using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Notification.Common.Contracts.Email;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendEventCancelledEmailConsumer : IConsumer<EventCancelledEmailEvent>
    {
        private readonly ILogger<SendEventCancelledEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;

        public SendEventCancelledEmailConsumer(
            ILogger<SendEventCancelledEmailConsumer> logger,
            MailServiceFactory mailServiceFactory)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
        }

        public async Task Consume(ConsumeContext<EventCancelledEmailEvent> context)
        {
            EventCancelledEmailEvent message = context.Message;

            _logger.LogInformation(
                "Received EventCancelledEmailEvent: EventId={EventId}, CorrelationId={CorrelationId}",
                message.EventId,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.OrganizerEmail,
                Subject = $"Sự kiện \"{message.EventTitle}\" đã bị hủy",
                HtmlBody = BuildEmailBody(message)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation("Event cancelled email sent for EventId={EventId}", message.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event cancelled email for EventId={EventId}", message.EventId);
                throw;
            }
        }

        private static string BuildEmailBody(EventCancelledEmailEvent message)
        {
            string reasonHtml = string.Empty;
            if (!string.IsNullOrWhiteSpace(message.Reason))
            {
                reasonHtml = $@"
                <div style='margin-top:20px; padding:15px; background-color:#f8d7da; color:#721c24; border:1px solid #f5c6cb; border-radius:5px;'>
                    <strong>Lý do hủy:</strong><br/>
                    {message.Reason}
                </div>";
            }

            return $@"
        <div style='font-family:Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Xin chào {message.OrganizerName},</h2>

            <p>Sự kiện <strong>""{message.EventTitle}""</strong> của bạn đã bị hủy.</p>

            {reasonHtml}

            <p>Toàn bộ đơn hàng chưa check-in của sự kiện này sẽ được tự động hoàn tiền cho khách hàng.</p>
        </div>
    ";
        }
    }
}
