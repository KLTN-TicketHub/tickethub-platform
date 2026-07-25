using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Notification.Common.Contracts.Email;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendOrderRefundedEmailConsumer : IConsumer<OrderRefundedEmailEvent>
    {
        private readonly ILogger<SendOrderRefundedEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;

        public SendOrderRefundedEmailConsumer(
            ILogger<SendOrderRefundedEmailConsumer> logger,
            MailServiceFactory mailServiceFactory)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
        }

        public async Task Consume(ConsumeContext<OrderRefundedEmailEvent> context)
        {
            OrderRefundedEmailEvent message = context.Message;

            _logger.LogInformation(
                "Received OrderRefundedEmailEvent: OrderId={OrderId}, CorrelationId={CorrelationId}",
                message.OrderId,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.CustomerEmail,
                Subject = $"Đơn hàng của bạn đã được hoàn tiền do sự kiện \"{message.EventTitle}\" bị hủy",
                HtmlBody = BuildEmailBody(message)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation("Order refunded email sent for OrderId={OrderId}", message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send order refunded email for OrderId={OrderId}", message.OrderId);
                throw;
            }
        }

        private static string BuildEmailBody(OrderRefundedEmailEvent message)
        {
            return $@"
        <div style='font-family:Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Xin chào {message.CustomerName},</h2>

            <p>Sự kiện <strong>""{message.EventTitle}""</strong> đã bị hủy, đơn hàng <strong>{message.OrderId}</strong> của bạn đã được hoàn tiền.</p>

            <p>Số tiền hoàn: <strong>{message.RefundedAmount:N0} VND</strong></p>
            <p>Thời gian hoàn tiền: {message.RefundedAt:dd/MM/yyyy HH:mm}</p>

            <p>Tiền sẽ được hoàn về đúng phương thức thanh toán bạn đã sử dụng khi đặt vé.</p>
        </div>
    ";
        }
    }
}
