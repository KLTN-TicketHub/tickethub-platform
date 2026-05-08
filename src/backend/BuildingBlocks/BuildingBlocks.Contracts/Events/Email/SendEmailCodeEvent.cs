namespace BuildingBlocks.Contracts.Events.Email
{
    public record SendEmailCodeEvent
    {
        public string UserId { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string FullName { get; init; } = string.Empty;

        public string Code { get; init; } = string.Empty;

        public DateTime ExpiresAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "AdminLogin";
    }
}
