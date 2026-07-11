namespace Notification.Common.Contracts.Email
{
    public sealed record EmailNotificationRequest
    {
        public string To { get; init; } = string.Empty;

        public string Subject { get; init; } = string.Empty;

        public string HtmlBody { get; init; } = string.Empty;

        public string? TextBody { get; init; }

        public List<EmailAttachmentDto> Attachments { get; init; } = new();
    }
}