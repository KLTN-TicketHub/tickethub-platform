namespace BuildingBlocks.Contracts.Events.Email
{
    public class OrganizerRegisteredEvent
    {
        public string UserId { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string FullName { get; init; } = string.Empty;

        public string UserName { get; init; } = string.Empty;

        public string ActivationToken { get; init; } = string.Empty;

        public DateTime ExpiresAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "OrganizerRegistered";
    }
}
