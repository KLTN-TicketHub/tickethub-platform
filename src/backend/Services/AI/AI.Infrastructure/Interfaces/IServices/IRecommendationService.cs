namespace AI.Infrastructure.Interfaces.IServices
{
    public class RecommendationItemResult
    {
        public Guid EventId { get; set; }
        public double Score { get; set; }
        public int Rank { get; set; }
    }

    public class TrainingStatusResult
    {
        public DateTime? LastTrainedAt { get; set; }
        public int InteractionCount { get; set; }
        public int UserWithRecommendationCount { get; set; }
    }

    public interface IRecommendationService
    {
        Task SyncInteractionsAsync(CancellationToken cancellation = default);

        Task TrainModelAsync(bool evaluateOnly = false, CancellationToken cancellation = default);

        Task SyncAndTrainAsync(CancellationToken cancellation = default);

        Task<List<RecommendationItemResult>> GetRecommendationsAsync(Guid userId, int topN = 10, CancellationToken cancellation = default);

        Task<TrainingStatusResult> GetTrainingStatusAsync(CancellationToken cancellation = default);
    }
}
