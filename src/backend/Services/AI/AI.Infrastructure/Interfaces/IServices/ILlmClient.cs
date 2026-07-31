namespace AI.Infrastructure.Interfaces.IServices
{
    public class ChatHistoryMessage
    {
        public string Role { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }

    public interface ILlmClient
    {
        Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);

        Task<string> ChatCompleteAsync(string systemPrompt, IReadOnlyList<ChatHistoryMessage> history, CancellationToken cancellationToken = default);
    }
}
