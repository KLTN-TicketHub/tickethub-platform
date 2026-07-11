namespace Notification.Common.Contracts.Email
{
    public sealed record EmailAttachmentDto
    {
        public string ContentId { get; init; } = string.Empty;
        public byte[] ContentBytes { get; init; } = Array.Empty<byte>();
        public string ContentType { get; init; } = string.Empty;
    }
}
