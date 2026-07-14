using BuildingBlocks.Contracts.Events.Email;
using MassTransit;
using Microsoft.Extensions.Options;
using Notification.Common.Contracts.Email;
using Notification.Common.Options;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Worker.Consumers
{
    public class SendPayoutProposedEmailConsumer : IConsumer<PayoutProposedEmailEvent>
    {
        private readonly ILogger<SendPayoutProposedEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;
        private readonly AppUrls _appUrls;

        public SendPayoutProposedEmailConsumer(
            ILogger<SendPayoutProposedEmailConsumer> logger,
            MailServiceFactory mailServiceFactory,
            IOptions<AppUrls> appUrlsOptions)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
            _appUrls = appUrlsOptions.Value;
        }

        public async Task Consume(ConsumeContext<PayoutProposedEmailEvent> context)
        {
            PayoutProposedEmailEvent message = context.Message;

            _logger.LogInformation(
                "Received PayoutProposedEmailEvent: PayoutId={PayoutId}, EventId={EventId}, CorrelationId={CorrelationId}",
                message.PayoutId,
                message.EventId,
                message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.OrganizerEmail,
                Subject = $"Đề xuất giải ngân cho sự kiện \"{message.EventTitle}\"",
                HtmlBody = BuildPayoutProposedEmailBody(message, _appUrls.FrontendBaseUrl ?? string.Empty)
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Payout proposed email sent for PayoutId={PayoutId}, CorrelationId={CorrelationId}",
                    message.PayoutId,
                    message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send payout proposed email for PayoutId={PayoutId}", message.PayoutId);
                throw;
            }
        }

        private static string BuildPayoutProposedEmailBody(PayoutProposedEmailEvent message, string frontendBaseUrl)
        {
            string payoutDetailUrl = $"{frontendBaseUrl.TrimEnd('/')}/organizer/wallet";

            return $@"
        <div style='font-family:Arial, sans-serif; line-height: 1.6; color: #333;'>
            <h2>Xin chào {message.OrganizerName},</h2>

            <p>Moderator vừa đề xuất giải ngân cho sự kiện <strong>""{message.EventTitle}""</strong> của bạn.</p>

            <table style='margin-top:20px; border-collapse: collapse; width:100%;'>
                <tr>
                    <td style='padding:8px; border:1px solid #ddd;'>Doanh thu gộp</td>
                    <td style='padding:8px; border:1px solid #ddd;'>{message.GrossAmount:N0} đ</td>
                </tr>
                <tr>
                    <td style='padding:8px; border:1px solid #ddd;'>Phần trăm hoa hồng áp dụng</td>
                    <td style='padding:8px; border:1px solid #ddd;'>{message.AppliedRate}%</td>
                </tr>
                <tr>
                    <td style='padding:8px; border:1px solid #ddd;'>Phí hoa hồng</td>
                    <td style='padding:8px; border:1px solid #ddd;'>{message.FeeAmount:N0} đ</td>
                </tr>
                <tr>
                    <td style='padding:8px; border:1px solid #ddd; font-weight:bold;'>Số tiền thực nhận</td>
                    <td style='padding:8px; border:1px solid #ddd; font-weight:bold;'>{message.NetAmount:N0} đ</td>
                </tr>
            </table>

            <p style='margin-top:20px;'>Vui lòng đăng nhập để xem chi tiết và xác nhận chấp nhận đề xuất. Số tiền chỉ được cộng vào ví sau khi bạn xác nhận.</p>

            <p style='margin-top: 30px;'>
                <a href='{payoutDetailUrl}'
                   style='padding:12px 20px; background:#007bff; color:#fff; text-decoration:none; border-radius:5px; font-weight:bold;'>
                   Xem đề xuất giải ngân
                </a>
            </p>

            <p style='margin-top:40px; color:gray; font-size: 12px;'>
                Nếu nút bấm không hoạt động, bạn có thể copy đường link này vào trình duyệt:<br/>
                <a href='{payoutDetailUrl}'>{payoutDetailUrl}</a>
            </p>
        </div>
    ";
        }
    }
}
