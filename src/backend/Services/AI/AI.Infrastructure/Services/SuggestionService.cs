using AI.Common.Dtos.Suggestions;
using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AI.Infrastructure.Services
{
    public class SuggestionService : ISuggestionService
    {
        private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(12);

        private const string SystemPrompt =
            "Bạn là trợ lý phân tích dữ liệu cho một nền tảng bán vé sự kiện trực tuyến. " +
            "Nhiệm vụ của bạn là đọc các số liệu đã được tính toán sẵn (không tự tính toán lại số liệu) " +
            "và đưa ra tối đa 4 gợi ý hướng phát triển ngắn gọn, thực tế cho Organizer (đơn vị tổ chức sự kiện). " +
            "Luôn trả lời bằng tiếng Việt, giọng văn lịch sự. " +
            "Bắt buộc trả về ĐÚNG định dạng JSON sau, không kèm theo bất kỳ text nào khác ngoài JSON: " +
            "{\"summary\": string, \"suggestions\": [{\"category\": string, \"title\": string, \"detail\": string, \"basedOn\": string}]}.";

        private readonly ICatalogAiClient _catalogAiClient;
        private readonly ILlmClient _llmClient;
        private readonly ICacheService _cacheService;
        private readonly ILogger<SuggestionService> _logger;

        public SuggestionService(
            ICatalogAiClient catalogAiClient,
            ILlmClient llmClient,
            ICacheService cacheService,
            ILogger<SuggestionService> logger)
        {
            _catalogAiClient = catalogAiClient;
            _llmClient = llmClient;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetMarketDirectionSuggestionsAsync(
            Guid organizerId, string range, CancellationToken cancellationToken = default)
        {
            (DateOnly from, DateOnly to) = ResolveDateRange(range);

            CategoryTrendResult trend = await _catalogAiClient.GetCategoryTrendAsync(from, to);
            if (!trend.IsSuccess)
                return (false, trend.Message, null);

            OrganizerPortfolioResult portfolio = await _catalogAiClient.GetOrganizerPortfolioAsync(organizerId, from, to);

            var promptData = new
            {
                period = range,
                marketTrend = trend.Categories.Select(c => new
                {
                    category = c.CategoryName,
                    viewCount = c.ViewCount,
                    purchaseIntentCount = c.PurchaseIntentCount,
                    conversionRate = c.ViewCount > 0 ? Math.Round(c.PurchaseIntentCount * 100.0 / c.ViewCount, 1) : 0,
                    viewGrowthPercent = c.ViewGrowthPercent,
                    activeEventCount = c.ActiveEventCount,
                    avgOverallRating = c.AvgOverallRating,
                    ratingSampleSize = c.RatingSampleSize
                }),
                organizerOwnCategoryShare = portfolio.IsSuccess
                    ? portfolio.CategoryDistribution.Select(c => new { category = c.CategoryName, eventCount = c.EventCount })
                    : Enumerable.Empty<object>()
            };

            return await GetOrBuildSuggestionAsync("market-direction", organizerId.ToString(), promptData, cancellationToken);
        }

        public async Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetOrganizerPortfolioSuggestionsAsync(
            Guid organizerId, string range, CancellationToken cancellationToken = default)
        {
            (DateOnly from, DateOnly to) = ResolveDateRange(range);

            OrganizerPortfolioResult portfolio = await _catalogAiClient.GetOrganizerPortfolioAsync(organizerId, from, to);
            if (!portfolio.IsSuccess)
                return (false, portfolio.Message, null);

            var promptData = new
            {
                period = range,
                categoryDistribution = portfolio.CategoryDistribution.Select(c => new { category = c.CategoryName, eventCount = c.EventCount }),
                events = portfolio.Events.Select(e => new
                {
                    eventTitle = e.EventTitle,
                    viewCount = e.ViewCount,
                    purchaseIntentCount = e.PurchaseIntentCount,
                    conversionRate = e.ConversionRate,
                    overallRatingAvg = e.OverallRatingAvg,
                    ratingSampleSize = e.RatingSampleSize,
                    lowestRatingDimension = e.RatingSampleSize > 0 ? e.LowestRatingDimension : null
                })
            };

            return await GetOrBuildSuggestionAsync("portfolio", organizerId.ToString(), promptData, cancellationToken);
        }

        public async Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetEventSuggestionsAsync(
            Guid organizerId, Guid eventId, string range, CancellationToken cancellationToken = default)
        {
            (DateOnly from, DateOnly to) = ResolveDateRange(range);

            EventInsightResult insight = await _catalogAiClient.GetEventInsightAsync(eventId, organizerId, from, to);
            if (!insight.IsSuccess)
                return (false, insight.Message, null);

            var promptData = new
            {
                eventTitle = insight.EventTitle,
                period = range,
                metrics = new
                {
                    viewCount = insight.ViewCount,
                    purchaseIntentCount = insight.PurchaseIntentCount,
                    conversionRate = insight.ConversionRate
                },
                ratings = insight.RatingSampleSize > 0
                    ? new
                    {
                        overallAvg = insight.OverallAvg,
                        sampleSize = insight.RatingSampleSize,
                        byDimension = new
                        {
                            sound = insight.SoundAvg,
                            visual = insight.VisualAvg,
                            organization = insight.OrganizationAvg,
                            facility = insight.FacilityAvg,
                            service = insight.ServiceAvg,
                            performance = insight.PerformanceAvg
                        }
                    }
                    : null,
                sampleComments = insight.RecentComments
            };

            return await GetOrBuildSuggestionAsync($"event:{eventId}", organizerId.ToString(), promptData, cancellationToken);
        }

        private async Task<(bool IsSuccess, string Message, AiSuggestionResultDto? Data)> GetOrBuildSuggestionAsync(
            string suggestionType, string scopeId, object promptData, CancellationToken cancellationToken)
        {
            string promptJson = JsonSerializer.Serialize(promptData);
            string cacheKey = $"ai-suggestion:{suggestionType}:{scopeId}:{ComputeHash(promptJson)}";

            AiSuggestionResultDto? cached = await _cacheService.GetAsync<AiSuggestionResultDto>(cacheKey, cancellationToken);
            if (cached != null)
                return (true, "Thành công (cache).", cached);

            try
            {
                string rawResponse = await _llmClient.CompleteAsync(SystemPrompt, promptJson, cancellationToken);
                AiSuggestionResultDto result = ParseSuggestionResult(rawResponse);

                await _cacheService.SetAsync(cacheKey, result, CacheTtl, cancellationToken);

                return (true, "Thành công.", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SuggestionService] Không thể tạo gợi ý AI cho {SuggestionType}/{ScopeId}.", suggestionType, scopeId);
                return (false, "Không thể tạo gợi ý AI lúc này. Vui lòng thử lại sau.", null);
            }
        }

        private static AiSuggestionResultDto ParseSuggestionResult(string rawResponse)
        {
            try
            {
                string jsonText = ExtractJsonPayload(rawResponse);

                AiSuggestionResultDto? parsed = JsonSerializer.Deserialize<AiSuggestionResultDto>(
                    jsonText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed != null && parsed.Suggestions.Count > 0)
                    return parsed;
            }
            catch (JsonException)
            {
            }

            return new AiSuggestionResultDto
            {
                Summary = "Không thể phân tích gợi ý chi tiết, dưới đây là nội dung gốc từ AI.",
                Suggestions = new List<SuggestionItemDto>
                {
                    new SuggestionItemDto
                    {
                        Category = "general",
                        Title = "Gợi ý từ AI",
                        Detail = rawResponse,
                        BasedOn = "raw"
                    }
                }
            };
        }

        private static string ExtractJsonPayload(string rawResponse)
        {
            string text = rawResponse.Trim();

            if (text.StartsWith("```"))
            {
                int firstNewline = text.IndexOf('\n');
                if (firstNewline >= 0)
                    text = text[(firstNewline + 1)..];

                int fenceEnd = text.LastIndexOf("```");
                if (fenceEnd >= 0)
                    text = text[..fenceEnd];

                text = text.Trim();
            }

            int start = text.IndexOf('{');
            int end = text.LastIndexOf('}');
            if (start >= 0 && end > start)
                return text[start..(end + 1)];

            return text;
        }

        private static (DateOnly From, DateOnly To) ResolveDateRange(string range)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            int days = range == "7d" ? 7 : 30;
            return (today.AddDays(-(days - 1)), today);
        }

        private static string ComputeHash(string input)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
