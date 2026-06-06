using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Notification.Common.Contracts.Email;
using Notification.Infrastructure.Factories.Email;

namespace Notification.Worker.Consumers
{
    public class SendModeratorWelcomeEmailConsumer : IConsumer<ModeratorRegisteredEvent>
    {
        private readonly ILogger<SendModeratorWelcomeEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;

        public SendModeratorWelcomeEmailConsumer(
            ILogger<SendModeratorWelcomeEmailConsumer> logger,
            MailServiceFactory mailServiceFactory)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
        }

        public async Task Consume(ConsumeContext<ModeratorRegisteredEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received ModeratorRegisteredEvent: UserId={UserId}, Email={Email}, CorrelationId={CorrelationId}",
                message.UserId,
                message.Email,
                message.CorrelationId);

            var mailService = _mailServiceFactory.Create();

            var emailRequest = new EmailNotificationRequest
            {
                To = message.Email,
                Subject = "Chào mừng bạn đến với TicketHub — Thông tin tài khoản Moderator",
                HtmlBody = BuildWelcomeEmailBody(message)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Moderator welcome email sent for UserId={UserId}, CorrelationId={CorrelationId}",
                    message.UserId,
                    message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send moderator welcome email for UserId={UserId}", message.UserId);
                throw;
            }
        }

        private static string BuildWelcomeEmailBody(ModeratorRegisteredEvent message)
        {
            string activationUrl =
                $"http://localhost:5173/activate-account" +
                $"?userId={message.UserId}" +
                $"&token={Uri.EscapeDataString(message.ActivationToken)}";

            return $"""
<div style="font-family: Arial, sans-serif; line-height: 1.6; color: #111827; max-width: 600px; margin: 0 auto;">

    <h2 style="margin-bottom: 16px;">Chào mừng đến với TicketHub</h2>

    <p>Xin chào <strong>{message.FullName}</strong>,</p>

    <p>Tài khoản <strong>Moderator</strong> của bạn đã được tạo trên hệ thống TicketHub.</p>

    <hr style="margin: 20px 0;" />

    <p><strong>Thông tin tài khoản:</strong></p>
    <p>Username: {message.UserName}</p>
    <p>Tên đăng nhập: {message.UserName}</p>
    <p>Email: {message.Email}</p>

    <p style="margin-top: 20px;">
        Để kích hoạt tài khoản và thiết lập mật khẩu, vui lòng nhấn vào liên kết bên dưới:
    </p>

    <p style="margin: 20px 0;">
        <a href="{activationUrl}">Kích hoạt tài khoản</a>
    </p>

    <p>
        Liên kết này có hiệu lực đến: {message.ExpiredAt:dd/MM/yyyy HH:mm:ss} UTC
    </p>

    <p style="margin-top: 20px;">
        Nếu bạn không yêu cầu email này, vui lòng bỏ qua.
    </p>

    <p style="margin-top: 30px;">
        Trân trọng,<br/>
        TicketHub
    </p>

</div>
""";
        }
    }
}
