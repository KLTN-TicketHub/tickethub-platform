using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Notification.Common.Contracts.Email;
using Notification.Common.Options;
using Notification.Infrastructure.Interfaces.IEmail;

namespace Notification.Infrastructure.Services.Email
{
    public sealed class MailService : IMailService
    {
        private readonly EmailSettings _emailSettings;

        public MailService(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task SendAsync(EmailNotificationRequest email, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpHost))
                throw new InvalidOperationException("SMTP host is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.SmtpUser))
                throw new InvalidOperationException("SMTP user is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.Password))
                throw new InvalidOperationException("SMTP password is not configured.");

            if (string.IsNullOrWhiteSpace(_emailSettings.From))
                throw new InvalidOperationException("SMTP from address is not configured.");

            if (string.IsNullOrWhiteSpace(email.To))
                throw new ArgumentException("Email recipient is required.", nameof(email));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.DisplayName ?? string.Empty, _emailSettings.From));
            message.To.Add(MailboxAddress.Parse(email.To));
            message.Subject = email.Subject;

            var builder = new BodyBuilder
            {
                HtmlBody = email.HtmlBody,
                TextBody = email.TextBody
            };

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls, cancellation);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.Password, cancellation);
            await smtp.SendAsync(message, cancellation);
            await smtp.DisconnectAsync(true, cancellation);
        }
    }
}