using BuildingBlocks.Contracts.Events.Inventory;
using MassTransit;
using Microsoft.Extensions.Options;
using Notification.Common.Contracts.Email;
using Notification.Common.Options;
using Notification.Infrastructure.Factories.Email;
using Notification.Infrastructure.Interfaces.IEmail;
using QRCoder;

namespace Notification.Worker.Consumers
{
    public class SendTicketEmailConsumer : IConsumer<TicketsIssuedEvent>
    {
        private readonly ILogger<SendTicketEmailConsumer> _logger;
        private readonly MailServiceFactory _mailServiceFactory;
        private readonly AppUrls _appUrls;

        public SendTicketEmailConsumer(
            ILogger<SendTicketEmailConsumer> logger,
            MailServiceFactory mailServiceFactory,
            IOptions<AppUrls> appUrlsOptions)
        {
            _logger = logger;
            _mailServiceFactory = mailServiceFactory;
            _appUrls = appUrlsOptions.Value;
        }

        public async Task Consume(ConsumeContext<TicketsIssuedEvent> context)
        {
            TicketsIssuedEvent message = context.Message;

            _logger.LogInformation(
                "Sending ticket email for OrderId={OrderId} to {Email}, CorrelationId={CorrelationId}",
                message.OrderId, message.CustomerEmail, message.CorrelationId);

            IMailService mailService = _mailServiceFactory.Create();

            string subject = $"🎟️ Vé sự kiện \"{message.EventTitle}\" của bạn đã sẵn sàng!";

            var attachments = new List<EmailAttachmentDto>();
            _logger.LogInformation("Processing {Count} tickets, generating QR codes locally...", message.Tickets.Count);
            foreach (var ticket in message.Tickets)
            {
                try
                {
                    byte[] qrBytes = GenerateQrPngBytes(ticket.QrCodeToken);
                    _logger.LogInformation("Generated QR PNG for Token={Token}, size={Size} bytes", ticket.QrCodeToken, qrBytes.Length);
                    attachments.Add(new EmailAttachmentDto
                    {
                        ContentId = $"qr_{ticket.QrCodeToken}@tickethub.platform",
                        ContentBytes = qrBytes,
                        ContentType = "image/png"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate QR PNG for ticket token: {Token}", ticket.QrCodeToken);
                }
            }

            _logger.LogInformation("Added {Count} QR attachments to email", attachments.Count);

            EmailNotificationRequest emailRequest = new EmailNotificationRequest
            {
                To = message.CustomerEmail,
                Subject = subject,
                HtmlBody = BuildTicketEmailBody(message, _appUrls.FrontendBaseUrl ?? string.Empty),
                Attachments = attachments
            };

            try
            {
                await mailService.SendAsync(emailRequest, context.CancellationToken);

                _logger.LogInformation(
                    "Ticket email sent for OrderId={OrderId}, CorrelationId={CorrelationId}",
                    message.OrderId, message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send ticket email for OrderId={OrderId}", message.OrderId);
                throw;
            }
        }

        private static byte[] GenerateQrPngBytes(string qrToken)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(qrToken, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrData);
            return qrCode.GetGraphic(10);
        }

        private static string BuildTicketEmailBody(TicketsIssuedEvent message, string frontendBaseUrl)
        {
            string eventUrl = $"{frontendBaseUrl.TrimEnd('/')}/events/{message.EventId}";
            string formattedDate = message.ShowtimeStartAt.ToString("dd/MM/yyyy HH:mm") + " UTC";

            string ticketsHtml = string.Join("\n", message.Tickets.Select((ticket, index) =>
            {
                string seatInfo = !string.IsNullOrEmpty(ticket.SeatName)
                    ? $"Khu vực: <strong>{ticket.TicketTypeName}</strong><br/>Ghế: <strong>{ticket.SeatName}</strong>" + (!string.IsNullOrEmpty(ticket.RowName) ? $" - Hàng <strong>{ticket.RowName}</strong>" : "")
                    : $"Khu vực: <strong>{ticket.TicketTypeName}</strong><br/>Vé vào cổng (Standing)";

                return $@"
                <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background:#ffffff; border:1px solid #e2e8f0; border-radius:12px; margin-bottom:20px; overflow:hidden; border-left:4px solid #0f172a;'>
                    <tr>
                        <td style='padding:24px; vertical-align:top;'>
                            <div style='font-family:-apple-system,BlinkMacSystemFont,""Segoe UI"",Roboto,sans-serif;'>
                                <div style='font-size:11px; font-weight:700; text-transform:uppercase; letter-spacing:1px; color:#64748b; margin-bottom:6px;'>VÉ SỐ #{index + 1}</div>
                                <div style='font-size:18px; font-weight:800; color:#0f172a; margin-bottom:12px;'>{message.EventTitle}</div>
                                <div style='font-size:13px; color:#475569; line-height:1.6; margin-bottom:12px;'>
                                    {seatInfo}
                                </div>
                                <div style='font-size:16px; font-weight:800; color:#0f172a;'>{ticket.Price:N0} VNĐ</div>
                                <div style='font-size:11px; font-family:monospace; color:#94a3b8; margin-top:16px;'>MÃ SỐ: {ticket.QrCodeToken}</div>
                            </div>
                        </td>
                        <td width='1' style='border-left:1px dashed #cbd5e1;'></td>
                        <td width='150' style='padding:24px; text-align:center; vertical-align:middle; background:#fafafa;'>
                            <img src=""cid:qr_{ticket.QrCodeToken}@tickethub.platform"" 
                                 alt=""QR Code"" 
                                 width=""110"" 
                                 height=""110"" 
                                 style=""display:inline-block; border:1px solid #e2e8f0; border-radius:6px; padding:4px; background:#ffffff;"" />
                            <div style='font-family:-apple-system,BlinkMacSystemFont,""Segoe UI"",Roboto,sans-serif; font-size:10px; color:#64748b; margin-top:8px; font-weight:600; text-transform:uppercase; letter-spacing:0.5px;'>Quét tại cửa</div>
                        </td>
                    </tr>
                </table>";
            }));

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Vé của bạn đã sẵn sàng</title>
</head>
<body style='margin:0; padding:0; background-color:#f8fafc; font-family:-apple-system,BlinkMacSystemFont,""Segoe UI"",Roboto,sans-serif;-webkit-font-smoothing:antialiased;'>
    <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color:#f8fafc; padding:40px 20px;'>
        <tr>
            <td align='center'>
                <table cellpadding='0' cellspacing='0' border='0' width='100%' style='max-width:600px; background-color:#ffffff; border:1px solid #e2e8f0; border-radius:16px; overflow:hidden; box-shadow:0 4px 6px -1px rgba(0,0,0,0.05), 0 2px 4px -2px rgba(0,0,0,0.05);'>
                    <!-- Brand Header -->
                    <tr>
                        <td style='padding:32px 40px 24px 40px; border-bottom:1px solid #f1f5f9; text-align:center;'>
                            <div style='font-size:18px; font-weight:900; letter-spacing:4px; color:#0f172a; text-transform:uppercase;'>T I C K E T H U B</div>
                        </td>
                    </tr>
                    
                    <!-- Content Body -->
                    <tr>
                        <td style='padding:40px 40px 32px 40px;'>
                            <h1 style='margin:0 0 16px 0; font-size:24px; font-weight:800; tracking-tight; color:#0f172a;'>Vé của bạn đã sẵn sàng!</h1>
                            <p style='margin:0 0 24px 0; font-size:15px; line-height:1.6; color:#334155;'>
                                Xin chào <strong>{message.CustomerName}</strong>,<br/>
                                Yêu cầu đặt vé cho đơn hàng của bạn đã hoàn tất thành công. Dưới đây là thông tin chi tiết vé điện tử của bạn. Vui lòng xuất trình mã QR tương ứng để quét khi check-in tại lối vào sự kiện.
                            </p>

                            <!-- Event Metadata Card -->
                            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color:#f8fafc; border:1px solid #e2e8f0; border-radius:12px; margin-bottom:32px;'>
                                <tr>
                                    <td style='padding:20px;'>
                                        <table cellpadding='0' cellspacing='0' border='0' width='100%'>
                                            <tr>
                                                <td style='padding-bottom:12px;'>
                                                    <div style='font-size:11px; font-weight:700; text-transform:uppercase; letter-spacing:0.5px; color:#64748b; margin-bottom:4px;'>Sự kiện</div>
                                                    <div style='font-size:16px; font-weight:800; color:#0f172a;'>{message.EventTitle}</div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <div style='font-size:11px; font-weight:700; text-transform:uppercase; letter-spacing:0.5px; color:#64748b; margin-bottom:4px;'>Thời gian bắt đầu</div>
                                                    <div style='font-size:15px; font-weight:700; color:#0f172a;'>{formattedDate}</div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            <!-- Tickets Section -->
                            <h2 style='margin:0 0 16px 0; font-size:15px; font-weight:800; text-transform:uppercase; letter-spacing:1px; color:#0f172a; padding-bottom:8px; border-bottom:1px solid #f1f5f9;'>Danh sách vé phát hành</h2>
                            
                            {ticketsHtml}

                            <!-- Guidelines / Note -->
                            <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color:#fffbeb; border:1px solid #fef3c7; border-radius:12px; margin-top:32px; margin-bottom:32px;'>
                                <tr>
                                    <td style='padding:20px;'>
                                        <div style='font-size:13px; font-weight:800; color:#b45309; margin-bottom:8px; text-transform:uppercase; letter-spacing:0.5px;'>Lưu ý quan trọng</div>
                                        <ul style='margin:0; padding-left:20px; font-size:13px; color:#78350f; line-height:1.7;'>
                                            <li style='margin-bottom:6px;'>Mỗi mã QR tương ứng với 1 lượt check-in và chỉ có giá trị cho <strong>một lần quét duy nhất</strong>.</li>
                                            <li style='margin-bottom:6px;'>Không chia sẻ hình ảnh mã QR này với bất kỳ ai để tránh bị sử dụng trái phép.</li>
                                            <li>Nên lưu sẵn hình ảnh mã QR vào thiết bị di động đề phòng trường hợp mất kết nối mạng tại địa điểm diễn ra sự kiện.</li>
                                        </ul>
                                    </td>
                                </tr>
                            </table>

                            <!-- Call to Action -->
                            <table cellpadding='0' cellspacing='0' border='0' width='100%'>
                                <tr>
                                    <td align='center' style='padding-top:8px;'>
                                        <a href='{eventUrl}' style='display:inline-block; background-color:#0f172a; color:#ffffff; font-size:14px; font-weight:700; text-decoration:none; padding:14px 28px; border-radius:8px; box-shadow:0 1px 2px 0 rgba(0, 0, 0, 0.05); text-transform:uppercase; letter-spacing:0.5px;'>Quản lý vé của tôi</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='padding:24px 40px; background-color:#fafafa; border-top:1px solid #f1f5f9; text-align:center;'>
                            <div style='font-size:12px; color:#64748b; line-height:1.6;'>
                                Email này được gửi tự động từ hệ thống TicketHub.<br/>
                                Vui lòng không phản hồi trực tiếp email này. Nếu có bất kỳ câu hỏi nào, vui lòng truy cập trang trợ giúp của chúng tôi.
                            </div>
                            <div style='margin-top:16px; font-size:11px; color:#94a3b8; font-weight:500;'>
                                &copy; {DateTime.UtcNow.Year} TicketHub Platform. All rights reserved.
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
