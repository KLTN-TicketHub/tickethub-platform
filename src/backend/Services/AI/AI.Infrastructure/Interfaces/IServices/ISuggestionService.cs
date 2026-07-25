using AI.Common.Dtos.Suggestions;

namespace AI.Infrastructure.Interfaces.IServices
{
    public interface ISuggestionService
    {
        Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetMarketDirectionSuggestionsAsync(
            Guid organizerId, string range, CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetOrganizerPortfolioSuggestionsAsync(
            Guid organizerId, string range, CancellationToken cancellationToken = default);

        Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetEventSuggestionsAsync(
            Guid organizerId, Guid eventId, string range, CancellationToken cancellationToken = default);
    }
}
