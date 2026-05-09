using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Notification.Common.Contracts.Email;
using Notification.Infrastructure.Factories.Email;

namespace Notification.Worker.Consumers
{
    public class SendEmailCodeConsumer : IConsumer<SendEmailCodeEvent>
    {
        private readonly ILogger<SendEmailCodeConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;

        public SendEmailCodeConsumer(
            ILogger<SendEmailCodeConsumer> logger,
            MailServiceFactory mailServiceFactory)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
        }

        public async Task Consume(ConsumeContext<SendEmailCodeEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received SendEmailCodeEvent: UserId={UserId}, Email={Email}, Purpose={Purpose}",
                message.UserId,
                message.Email,
                message.Purpose);

            var mailService = _mailServiceFactory.Create();

            var emailRequest = new EmailNotificationRequest
            {
                To = message.Email,
                Subject = "Xác thực đăng nhập Admin TicketHub",
                HtmlBody = BuildVerificationEmailBody(message)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Email verification code sent for UserId={UserId}, CorrelationId={CorrelationId}",
                    message.UserId,
                    message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email code for user {UserId}", message.UserId);
                throw;
            }
        }

        private static string BuildVerificationEmailBody(SendEmailCodeEvent message)
        {
            return $"""
                <div style=\"font-family: Arial, sans-serif; line-height: 1.6; color: #1f2937;\">
                    <h2 style=\"margin-bottom: 16px;\">Xác thực đăng nhập quản trị TicketHub</h2>
                <p>Xin chào {message.FullName},</p>
                <p>Mã xác thực đăng nhập của bạn là:</p>
                <div style=\"font-size: 32px; font-weight: 700; letter-spacing: 6px; margin: 24px 0;\">
                    {message.Code}
                </div>
                <p>
                    Mã này sẽ hết hạn vào lúc 
                    <strong>{message.ExpiresAt:yyyy-MM-dd HH:mm:ss} UTC</strong>.
                </p>
                <p>
                    Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.
                </p>
                <p>Trân trọng,<br/>TicketHub</p>
                </div>
                """;
        }
    }
}
