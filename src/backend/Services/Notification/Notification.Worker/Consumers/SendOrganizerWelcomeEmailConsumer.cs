using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Microsoft.Extensions.Options;
using Notification.Common.Contracts.Email;
using Notification.Common.Options;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendOrganizerWelcomeEmailConsumer : IConsumer<OrganizerRegisteredEvent>
    {
        private readonly ILogger<SendOrganizerWelcomeEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;
        private readonly AppUrls _appUrls;

        public SendOrganizerWelcomeEmailConsumer(
            ILogger<SendOrganizerWelcomeEmailConsumer> logger,
            MailServiceFactory mailServiceFactory,
            IOptions<AppUrls> appUrlsOptions)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
            _appUrls = appUrlsOptions.Value;
        }

        public async Task Consume(ConsumeContext<OrganizerRegisteredEvent> context)
        {
            OrganizerRegisteredEvent message = context.Message;

            _logger.LogInformation(
                "Received OrganizerRegisteredEvent: UserId={UserId}, Email={Email}, CorrelationId={CorrelationId}",
                message.UserId,
                message.Email,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.Email,
                Subject = "Chào mừng bạn đến với TicketHub — Thông tin tài khoản Organizer",
                HtmlBody = BuildWelcomeEmailBody(message, _appUrls.BackendUrl!)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Organizer welcome email sent for UserId={UserId}, CorrelationId={CorrelationId}",
                    message.UserId,
                    message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send organizer welcome email for UserId={UserId}", message.UserId);
                throw;
            }
        }

        private static string BuildWelcomeEmailBody(OrganizerRegisteredEvent message, string beUrl)
        {
            string baseUrl = $"{beUrl}";

            var confirmLink =
                $"{baseUrl}/api/v1/auth/organizer/confirm-email?userId={message.UserId}&token={Uri.EscapeDataString(message.ActivationToken)}";

            return $@"
        <div style='font-family:Arial'>
            <h2>Xin chào {message.FullName}</h2>

            <p>Bạn đã đăng ký tài khoản Organizer thành công.</p>

            <p>Vui lòng xác nhận email để kích hoạt tài khoản:</p>

            <a href='{confirmLink}'
               style='padding:10px 16px;background:#28a745;color:#fff;text-decoration:none;border-radius:5px'>
               Xác nhận tài khoản
            </a>

            <p style='margin-top:20px;color:gray'>
                Link có hiệu lực trong 15 phút.
            </p>
        </div>
    ";
        }
    }
}
