namespace AI.Common.Dtos.Chat
{
    public class ChatResponseDto
    {
        public Guid SessionId { get; set; }

        public string Reply { get; set; } = string.Empty;
    }
}
