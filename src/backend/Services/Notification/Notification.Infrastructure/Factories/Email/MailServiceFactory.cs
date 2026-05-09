using Microsoft.Extensions.Options;
using Notification.Common.Options;
using Notification.Infrastructure.Interfaces.IEmail;
using Notification.Infrastructure.Services.Email;

namespace Notification.Infrastructure.Factories.Email
{
    public sealed class MailServiceFactory
    {
        private readonly IOptions<EmailSettings> _emailOptions;

        public MailServiceFactory(IOptions<EmailSettings> emailOptions)
        {
            _emailOptions = emailOptions;
        }

        public IMailService Create()
        {
            return new MailService(_emailOptions);
        }
    }
}