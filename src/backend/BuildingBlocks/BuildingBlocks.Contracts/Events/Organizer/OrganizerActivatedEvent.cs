using System;

namespace BuildingBlocks.Contracts.Events.Organizer
{
    public class OrganizerActivatedEvent
    {
        public Guid Id { get; init; }

        public string FullName { get; init; } = string.Empty;

        public string? ImageUrl { get; init; }

        public string Email { get; init; }

        public string? PhoneNumber { get; init; }

        public DateTime CreatedAt { get; init; }

        public string CorrelationId { get; init; } = Guid.NewGuid().ToString();

        public string Purpose { get; init; } = "OrganizerActivated";
    }
}
