using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Notification.Worker.Consumers
{
    public class SendEmailCodeConsumer : IConsumer<SendEmailCodeEvent>
    {
        private readonly ILogger<SendEmailCodeConsumer> _logger;

        public SendEmailCodeConsumer(ILogger<SendEmailCodeConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SendEmailCodeEvent> context)
        {
            var message = context.Message;

            _logger.LogInformation(
                "Received SendEmailCodeEvent: UserId={UserId}, Email={Email}, Purpose={Purpose}",
                message.UserId,
                message.Email,
                message.Purpose);

            try
            {
                _logger.LogInformation(
                    "Email verification code: {Code}, Expires at: {ExpiresAt}",
                    message.Code,
                    message.ExpiresAt);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email code for user {UserId}", message.UserId);
                throw;
            }
        }
    }
}
