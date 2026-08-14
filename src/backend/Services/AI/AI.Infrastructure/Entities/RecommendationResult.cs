using BuildingBlocks.Domain.DDD;

namespace AI.Infrastructure.Entities
{
    public class RecommendationResult : BaseEntity, IAggregateRoot
    {
        public Guid UserId { get; set; }

        public Guid EventId { get; set; }

        public double Score { get; set; }

        public int Rank { get; set; }

        public DateTime GeneratedAt { get; set; }

        public RecommendationResult(Guid userId, Guid eventId, double score, int rank, DateTime generatedAt)
        {
            UserId = userId;
            EventId = eventId;
            Score = score;
            Rank = rank;
            GeneratedAt = generatedAt;
        }
    }
}
