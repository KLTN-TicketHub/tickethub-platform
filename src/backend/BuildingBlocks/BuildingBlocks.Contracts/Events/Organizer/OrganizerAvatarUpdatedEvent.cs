namespace BuildingBlocks.Contracts.Events.Organizer
{
    public class OrganizerAvatarUpdatedEvent
    {
        public Guid Id { get; init; }

        public string? ImageUrl { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "OrganizerAvatarUpdated";
    }
}
