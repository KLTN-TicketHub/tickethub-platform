namespace AI.Common.Dtos.Suggestions
{
    public class AiSuggestionResultDto
    {
        public string Summary { get; set; } = string.Empty;

        public List<SuggestionItemDto> Suggestions { get; set; } = new();
    }
}
