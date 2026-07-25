namespace AI.Infrastructure.Interfaces.IServices
{
    public interface ILlmClient
    {
        Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
    }
}
