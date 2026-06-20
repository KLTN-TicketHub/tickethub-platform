using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Microsoft.Extensions.Options;
using Notification.Common.Contracts.Email;
using Notification.Common.Options;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendEventReviewedEmailConsumer : IConsumer<EventReviewedEvent>
    {
        private readonly ILogger<SendEventReviewedEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;
        private readonly AppUrls _appUrls;

        public SendEventReviewedEmailConsumer(
            ILogger<SendEventReviewedEmailConsumer> logger,
            MailServiceFactory mailServiceFactory,
            IOptions<AppUrls> appUrlsOptions)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
            _appUrls = appUrlsOptions.Value;
        }

        public async Task Consume(ConsumeContext<EventReviewedEvent> context)
        {
            EventReviewedEvent message = context.Message;

            _logger.LogInformation(
                "Received EventReviewedEvent: EventId={EventId}, IsApproved={IsApproved}, CorrelationId={CorrelationId}",
                message.EventId,
                message.IsApproved,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            string statusText = message.IsApproved ? "Đã được phê duyệt" : "Bị từ chối";
            string subject = $"[{statusText}] Sự kiện \"{message.EventTitle}\"";

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.OrganizerEmail,
                Subject = subject,
                HtmlBody = BuildEventReviewedEmailBody(message, _appUrls.FrontendBaseUrl ?? string.Empty)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Event reviewed email sent for EventId={EventId}, CorrelationId={CorrelationId}",
                    message.EventId,
                    message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event reviewed email for EventId={EventId}", message.EventId);
                throw;
            }
        }

        private static string BuildEventReviewedEmailBody(EventReviewedEvent message, string frontendBaseUrl)
        {
            // baseUrl format e.g. http://localhost:5173
            string eventDetailUrl = $"{frontendBaseUrl.TrimEnd('/')}/organizer/events/{message.EventId}";
            
            string statusHtml = message.IsApproved 
                ? "<span style='color:#28a745; font-weight:bold;'>Đã được phê duyệt</span>" 
                : "<span style='color:#dc3545; font-weight:bold;'>Bị từ chối</span>";

            string reasonHtml = string.Empty;
            if (!message.IsApproved && !string.IsNullOrWhiteSpace(message.Reason))
            {
                reasonHtml = $@"
                <div style='margin-top:20px; padding:15px; background-color:#f8d7da; color:#721c24; border:1px solid #f5c6cb; border-radius:5px;'>
                    <strong>Lý do từ chối:</strong><br/>
                    {message.Reason}
                </div>";
            }

            string actionMessage = message.IsApproved 
                ? "Bạn có thể tiếp tục xem chi tiết và quản lý bán vé cho sự kiện của mình."
                : "Vui lòng xem lại lý do từ chối, chỉnh sửa lại thông tin và gửi yêu cầu phê duyệt lại.";

            return $@"
        <div style='font-family:Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Xin chào {message.OrganizerName},</h2>

            <p>Sự kiện <strong>""{message.EventTitle}""</strong> của bạn vừa được cập nhật trạng thái kiểm duyệt.</p>

            <p>Trạng thái hiện tại: {statusHtml}</p>

            {reasonHtml}

            <p>{actionMessage}</p>

            <p style='margin-top: 30px;'>
                <a href='{eventDetailUrl}'
                   style='padding:12px 20px; background:#007bff; color:#fff; text-decoration:none; border-radius:5px; font-weight:bold;'>
                   Xem chi tiết sự kiện
                </a>
            </p>

            <p style='margin-top:40px; color:gray; font-size: 12px;'>
                Nếu nút bấm không hoạt động, bạn có thể copy đường link này vào trình duyệt:<br/>
                <a href='{eventDetailUrl}'>{eventDetailUrl}</a>
            </p>
        </div>
    ";
        }
    }
}
