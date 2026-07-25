namespace AI.Common.Options
{
    public class AiOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string BaseUrl { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public int MaxTokens { get; set; } = 2048;

        public double Temperature { get; set; } = 0.7;

        public double AutoApproveConfidenceThreshold { get; set; } = 85.0;

        public int TimeoutSeconds { get; set; } = 30;

        public int MaxRetries { get; set; } = 3;
    }
}
