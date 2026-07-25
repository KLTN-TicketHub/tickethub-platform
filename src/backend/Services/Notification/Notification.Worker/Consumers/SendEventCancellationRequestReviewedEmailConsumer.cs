using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Notification.Common.Contracts.Email;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendEventCancellationRequestReviewedEmailConsumer : IConsumer<EventCancellationRequestReviewedEvent>
    {
        private readonly ILogger<SendEventCancellationRequestReviewedEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;

        public SendEventCancellationRequestReviewedEmailConsumer(
            ILogger<SendEventCancellationRequestReviewedEmailConsumer> logger,
            MailServiceFactory mailServiceFactory)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
        }

        public async Task Consume(ConsumeContext<EventCancellationRequestReviewedEvent> context)
        {
            EventCancellationRequestReviewedEvent message = context.Message;

            _logger.LogInformation(
                "Received EventCancellationRequestReviewedEvent: EventId={EventId}, IsApproved={IsApproved}, CorrelationId={CorrelationId}",
                message.EventId,
                message.IsApproved,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.OrganizerEmail,
                Subject = $"Yêu cầu hủy sự kiện \"{message.EventTitle}\" đã bị từ chối",
                HtmlBody = BuildEmailBody(message)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation("Event cancellation request reviewed email sent for EventId={EventId}", message.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event cancellation request reviewed email for EventId={EventId}", message.EventId);
                throw;
            }
        }

        private static string BuildEmailBody(EventCancellationRequestReviewedEvent message)
        {
            string reasonHtml = string.Empty;
            if (!string.IsNullOrWhiteSpace(message.Reason))
            {
                reasonHtml = $@"
                <div style='margin-top:20px; padding:15px; background-color:#f8d7da; color:#721c24; border:1px solid #f5c6cb; border-radius:5px;'>
                    <strong>Lý do từ chối:</strong><br/>
                    {message.Reason}
                </div>";
            }

            return $@"
        <div style='font-family:Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Xin chào {message.OrganizerName},</h2>

            <p>Yêu cầu hủy sự kiện <strong>""{message.EventTitle}""</strong> của bạn đã bị Moderator từ chối.</p>

            {reasonHtml}

            <p>Sự kiện của bạn vẫn đang hoạt động bình thường.</p>
        </div>
    ";
        }
    }
}
