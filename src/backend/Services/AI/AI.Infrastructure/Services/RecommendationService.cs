using AI.Infrastructure.Data.Contexts;
using AI.Infrastructure.Entities;
using AI.Infrastructure.Enums;
using AI.Infrastructure.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace AI.Infrastructure.Services
{
    public class RecommendationService : IRecommendationService
    {
        private const int TopN = 10;

        private readonly AiDbContext _dbContext;
        private readonly ICatalogAiClient _catalogAiClient;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(AiDbContext dbContext, ICatalogAiClient catalogAiClient, ILogger<RecommendationService> logger)
        {
            _dbContext = dbContext;
            _catalogAiClient = catalogAiClient;
            _logger = logger;
        }

        public async Task SyncInteractionsAsync(CancellationToken cancellation = default)
        {
            DateOnly from = new DateOnly(2020, 1, 1);
            DateOnly to = DateOnly.FromDateTime(DateTime.UtcNow);

            GetUserEventClicksResult clicksResult = await _catalogAiClient.GetUserEventClicksAsync(from, to);

            if (!clicksResult.IsSuccess)
            {
                _logger.LogWarning("Failed to fetch click data from Catalog: {Message}", clicksResult.Message);
                return;
            }

            List<UserEventInteraction> interactions = clicksResult.Clicks
                .GroupBy(c => new { c.UserId, c.EventId, c.ClickType })
                .Select(g => new UserEventInteraction(
                    userId: g.Key.UserId,
                    eventId: g.Key.EventId,
                    interactionType: Enum.Parse<InteractionType>(g.Key.ClickType),
                    weight: g.Count() * GetBaseWeight(g.Key.ClickType),
                    occurredAt: g.Max(c => c.ClickedAt)))
                .ToList();

            _dbContext.RemoveRange(_dbContext.UserEventInteractions);
            await _dbContext.UserEventInteractions.AddRangeAsync(interactions, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);

            _logger.LogInformation("Synced {Count} interaction records from Catalog.", interactions.Count);
        }

        public async Task TrainModelAsync(bool evaluateOnly = false, CancellationToken cancellation = default)
        {
            List<UserEventInteraction> interactions = await _dbContext.UserEventInteractions
                .AsNoTracking()
                .ToListAsync(cancellation);

            if (interactions.Count == 0)
            {
                _logger.LogWarning("No interaction data available to train the recommendation model.");
                return;
            }

            if (evaluateOnly)
            {
                EvaluateModel(interactions);
                return;
            }

            await TrainAndPersistAsync(interactions, cancellation);
        }

        private async Task TrainAndPersistAsync(List<UserEventInteraction> interactions, CancellationToken cancellation)
        {
            PredictionEngine<InteractionInput, InteractionPrediction> predictionEngine = TrainMatrixFactorization(interactions);

            Dictionary<Guid, HashSet<Guid>> interactedByUser = interactions
                .GroupBy(i => i.UserId)
                .ToDictionary(g => g.Key, g => g.Select(i => i.EventId).ToHashSet());

            List<Guid> allEvents = interactions.Select(i => i.EventId).Distinct().ToList();

            List<RecommendationResult> newResults = new List<RecommendationResult>();
            DateTime generatedAt = DateTime.UtcNow;

            foreach (Guid userId in interactedByUser.Keys)
            {
                List<RecommendationResult> userResults = PredictTopN(predictionEngine, userId, allEvents, interactedByUser[userId], TopN)
                    .Select((x, index) => new RecommendationResult(userId, x.EventId, x.Score, index + 1, generatedAt))
                    .ToList();

                newResults.AddRange(userResults);
            }

            _dbContext.RemoveRange(_dbContext.RecommendationResults);
            await _dbContext.RecommendationResults.AddRangeAsync(newResults, cancellation);
            await _dbContext.SaveChangesAsync(cancellation);

            _logger.LogInformation("Finished training the recommendation model, wrote {Count} results for {UserCount} users.",
                newResults.Count, interactedByUser.Count);
        }

        private void EvaluateModel(List<UserEventInteraction> allInteractions)
        {
            Random random = new Random(0);

            var byUser = allInteractions.GroupBy(i => i.UserId).ToList();

            List<UserEventInteraction> trainSet = new List<UserEventInteraction>();
            Dictionary<Guid, List<Guid>> testSetByUser = new Dictionary<Guid, List<Guid>>();

            foreach (var group in byUser)
            {
                List<UserEventInteraction> shuffled = group.OrderBy(_ => random.Next()).ToList();

                if (shuffled.Count < 2)
                {
                    trainSet.AddRange(shuffled);
                    continue;
                }

                int testCount = Math.Max(1, shuffled.Count / 5);
                testSetByUser[group.Key] = shuffled.Take(testCount).Select(i => i.EventId).ToList();
                trainSet.AddRange(shuffled.Skip(testCount));
            }

            if (testSetByUser.Count == 0)
            {
                _logger.LogWarning("Not enough data (each user needs >=2 interactions) to evaluate Precision@K/Recall@K.");
                return;
            }

            PredictionEngine<InteractionInput, InteractionPrediction> predictionEngine = TrainMatrixFactorization(trainSet);

            Dictionary<Guid, HashSet<Guid>> interactedByUserInTrain = trainSet
                .GroupBy(i => i.UserId)
                .ToDictionary(g => g.Key, g => g.Select(i => i.EventId).ToHashSet());

            List<Guid> candidateEvents = trainSet.Select(i => i.EventId).Distinct().ToList();

            Dictionary<Guid, double> popularityByEvent = trainSet
                .GroupBy(i => i.EventId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Weight));

            List<double> mfPrecisions = new List<double>();
            List<double> mfRecalls = new List<double>();
            List<double> popularityPrecisions = new List<double>();
            List<double> popularityRecalls = new List<double>();

            foreach ((Guid userId, List<Guid> groundTruth) in testSetByUser)
            {
                HashSet<Guid> alreadyInteracted = interactedByUserInTrain.GetValueOrDefault(userId, new HashSet<Guid>());

                List<Guid> mfTopK = PredictTopN(predictionEngine, userId, candidateEvents, alreadyInteracted, TopN)
                    .Select(x => x.EventId)
                    .ToList();

                List<Guid> popularityTopK = candidateEvents
                    .Where(eventId => !alreadyInteracted.Contains(eventId))
                    .OrderByDescending(eventId => popularityByEvent.GetValueOrDefault(eventId, 0))
                    .Take(TopN)
                    .ToList();

                int mfHits = mfTopK.Count(groundTruth.Contains);
                int popularityHits = popularityTopK.Count(groundTruth.Contains);

                mfPrecisions.Add((double)mfHits / TopN);
                mfRecalls.Add((double)mfHits / groundTruth.Count);
                popularityPrecisions.Add((double)popularityHits / TopN);
                popularityRecalls.Add((double)popularityHits / groundTruth.Count);
            }

            _logger.LogInformation(
                "Evaluated on {UserCount} users (K={TopN}): MatrixFactorization Precision@K={MfPrecision:F3} Recall@K={MfRecall:F3} | " +
                "Popularity baseline Precision@K={PopPrecision:F3} Recall@K={PopRecall:F3}",
                testSetByUser.Count, TopN,
                mfPrecisions.Average(), mfRecalls.Average(),
                popularityPrecisions.Average(), popularityRecalls.Average());
        }

        private PredictionEngine<InteractionInput, InteractionPrediction> TrainMatrixFactorization(List<UserEventInteraction> interactions)
        {
            MLContext mlContext = new MLContext(seed: 0);

            List<InteractionInput> trainingRows = interactions
                .Select(i => new InteractionInput
                {
                    UserId = i.UserId.ToString(),
                    EventId = i.EventId.ToString(),
                    Label = (float)i.Weight
                })
                .ToList();

            IDataView trainingData = mlContext.Data.LoadFromEnumerable(trainingRows);

            IEstimator<ITransformer> pipeline = mlContext.Transforms.Conversion
                .MapValueToKey(outputColumnName: "UserIdEncoded", inputColumnName: nameof(InteractionInput.UserId))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "EventIdEncoded", inputColumnName: nameof(InteractionInput.EventId)))
                .Append(mlContext.Recommendation().Trainers.MatrixFactorization(new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserIdEncoded",
                    MatrixRowIndexColumnName = "EventIdEncoded",
                    LabelColumnName = nameof(InteractionInput.Label),
                    NumberOfIterations = 20,
                    ApproximationRank = 32
                }));

            ITransformer model = pipeline.Fit(trainingData);

            return mlContext.Model.CreatePredictionEngine<InteractionInput, InteractionPrediction>(model);
        }

        private static List<(Guid EventId, double Score)> PredictTopN(
            PredictionEngine<InteractionInput, InteractionPrediction> predictionEngine,
            Guid userId,
            List<Guid> candidateEvents,
            HashSet<Guid> excludeEvents,
            int topN)
        {
            return candidateEvents
                .Where(eventId => !excludeEvents.Contains(eventId))
                .Select(eventId => (
                    EventId: eventId,
                    Score: (double)predictionEngine.Predict(new InteractionInput { UserId = userId.ToString(), EventId = eventId.ToString() }).Score))
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();
        }

        public async Task SyncAndTrainAsync(CancellationToken cancellation = default)
        {
            await SyncInteractionsAsync(cancellation);
            await TrainModelAsync(evaluateOnly: false, cancellation);
        }

        public async Task<List<RecommendationItemResult>> GetRecommendationsAsync(Guid userId, int topN = 10, CancellationToken cancellation = default)
        {
            List<RecommendationItemResult> results = await _dbContext.RecommendationResults
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Rank)
                .Take(topN)
                .Select(r => new RecommendationItemResult { EventId = r.EventId, Score = r.Score, Rank = r.Rank })
                .ToListAsync(cancellation);

            if (results.Count > 0)
                return results;

            return await GetPopularityFallbackAsync(topN, cancellation);
        }

        public async Task<TrainingStatusResult> GetTrainingStatusAsync(CancellationToken cancellation = default)
        {
            DateTime? lastTrainedAt = await _dbContext.RecommendationResults
                .AsNoTracking()
                .Select(r => (DateTime?)r.GeneratedAt)
                .OrderDescending()
                .FirstOrDefaultAsync(cancellation);

            int interactionCount = await _dbContext.UserEventInteractions.AsNoTracking().CountAsync(cancellation);

            int userWithRecommendationCount = await _dbContext.RecommendationResults
                .AsNoTracking()
                .Select(r => r.UserId)
                .Distinct()
                .CountAsync(cancellation);

            return new TrainingStatusResult
            {
                LastTrainedAt = lastTrainedAt,
                InteractionCount = interactionCount,
                UserWithRecommendationCount = userWithRecommendationCount
            };
        }

        private async Task<List<RecommendationItemResult>> GetPopularityFallbackAsync(int topN, CancellationToken cancellation)
        {
            List<UserEventInteraction> interactions = await _dbContext.UserEventInteractions
                .AsNoTracking()
                .ToListAsync(cancellation);

            return interactions
                .GroupBy(i => i.EventId)
                .Select(g => new { EventId = g.Key, TotalWeight = g.Sum(i => i.Weight) })
                .OrderByDescending(x => x.TotalWeight)
                .Take(topN)
                .Select((x, index) => new RecommendationItemResult { EventId = x.EventId, Score = x.TotalWeight, Rank = index + 1 })
                .ToList();
        }

        private static double GetBaseWeight(string clickType)
        {
            return clickType switch
            {
                nameof(InteractionType.PurchaseIntent) => 2,
                _ => 1
            };
        }
    }

    public class InteractionInput
    {
        public string UserId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public float Label { get; set; }
    }

    public class InteractionPrediction
    {
        public float Score { get; set; }
    }
}
