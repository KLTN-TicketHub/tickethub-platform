using AI.Common.Dtos.Chat;

namespace AI.Infrastructure.Interfaces.IServices
{
    public interface IChatService
    {
        Task<(bool IsSuccess, string Message, ChatResponseDto? Data)> SendMessageAsync(
            Guid userId, Guid? sessionId, string message, CancellationToken cancellationToken = default);
    }
}
