using Notification.Common.Contracts.Email;

namespace Notification.Infrastructure.Interfaces.IEmail
{
    public interface IMailService
    {
        Task SendAsync(EmailNotificationRequest email, CancellationToken cancellation = default);
    }
}