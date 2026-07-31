using AI.Common.Options;
using AI.Infrastructure.Interfaces.IServices;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AI.Infrastructure.Services
{
    public class OpenRouterLlmClient : ILlmClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AiOptions _options;

        public OpenRouterLlmClient(IHttpClientFactory httpClientFactory, IOptions<AiOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            OpenRouterRequest requestBody = new OpenRouterRequest
            {
                Model = _options.Model,
                Messages = new List<OpenRouterMessage>
                {
                    new OpenRouterMessage { Role = "system", Content = systemPrompt },
                    new OpenRouterMessage { Role = "user", Content = userPrompt }
                },
                Temperature = _options.Temperature,
                MaxTokens = _options.MaxTokens,
                ResponseFormat = new OpenRouterResponseFormat { Type = "json_object" }
            };

            Exception? lastException = null;

            for (int attempt = 1; attempt <= _options.MaxRetries; attempt++)
            {
                try
                {
                    return await SendRequestAsync(client, requestBody, cancellationToken);
                }
                catch (Exception ex) when (attempt < _options.MaxRetries)
                {
                    lastException = ex;
                }
            }

            throw lastException ?? new InvalidOperationException("Gọi OpenRouter API thất bại.");
        }

        public async Task<string> ChatCompleteAsync(string systemPrompt, IReadOnlyList<ChatHistoryMessage> history, CancellationToken cancellationToken = default)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

            List<OpenRouterMessage> messages = new List<OpenRouterMessage>
            {
                new OpenRouterMessage { Role = "system", Content = systemPrompt }
            };

            messages.AddRange(history.Select(m => new OpenRouterMessage { Role = m.Role, Content = m.Content }));

            OpenRouterRequest requestBody = new OpenRouterRequest
            {
                Model = _options.Model,
                Messages = messages,
                Temperature = _options.Temperature,
                MaxTokens = _options.MaxTokens
            };

            Exception? lastException = null;

            for (int attempt = 1; attempt <= _options.MaxRetries; attempt++)
            {
                try
                {
                    return await SendRequestAsync(client, requestBody, cancellationToken);
                }
                catch (Exception ex) when (attempt < _options.MaxRetries)
                {
                    lastException = ex;
                }
            }

            throw lastException ?? new InvalidOperationException("Gọi OpenRouter API thất bại.");
        }

        private async Task<string> SendRequestAsync(HttpClient client, OpenRouterRequest requestBody, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(_options.BaseUrl, requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"OpenRouter API trả về lỗi {response.StatusCode}: {errorBody}");
            }

            OpenRouterResponse? result = await response.Content.ReadFromJsonAsync<OpenRouterResponse>(cancellationToken: cancellationToken);

            string? text = result?.Choices?.FirstOrDefault()?.Message?.Content;

            if (string.IsNullOrWhiteSpace(text))
                throw new InvalidOperationException("OpenRouter API không trả về nội dung.");

            return text;
        }

        private class OpenRouterRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("messages")]
            public List<OpenRouterMessage> Messages { get; set; } = new();

            [JsonPropertyName("temperature")]
            public double Temperature { get; set; }

            [JsonPropertyName("max_tokens")]
            public int MaxTokens { get; set; }

            [JsonPropertyName("response_format")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public OpenRouterResponseFormat? ResponseFormat { get; set; }
        }

        private class OpenRouterMessage
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = string.Empty;

            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }

        private class OpenRouterResponseFormat
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = "json_object";
        }

        private class OpenRouterResponse
        {
            [JsonPropertyName("choices")]
            public List<OpenRouterChoice>? Choices { get; set; }
        }

        private class OpenRouterChoice
        {
            [JsonPropertyName("message")]
            public OpenRouterMessage? Message { get; set; }
        }
    }
}
