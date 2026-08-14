using AI.Infrastructure.Enums;
using BuildingBlocks.Domain.DDD;

namespace AI.Infrastructure.Entities
{
    public class UserEventInteraction : BaseEntity, IAggregateRoot
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public InteractionType InteractionType { get; set; }

        public double Weight { get; set; }

        public DateTime OccurredAt { get; set; }

        public UserEventInteraction(Guid userId, Guid eventId, InteractionType interactionType, double weight, DateTime occurredAt)
        {
            UserId = userId;
            EventId = eventId;
            InteractionType = interactionType;
            Weight = weight;
            OccurredAt = occurredAt;
        }
    }
}
