using AI.Common.Dtos.Chat;
using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AI.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private static readonly TimeSpan SessionTtl = TimeSpan.FromMinutes(45);
        private const int MaxHistoryMessages = 20;

        private const string ClassifyPrompt =
            "Bạn là bộ phân loại ý định cho chatbot hỗ trợ khách hàng của một nền tảng bán vé sự kiện trực tuyến. " +
            "Đọc tin nhắn của khách và phân loại vào ĐÚNG MỘT trong các intent sau: " +
            "\"event_info\" (hỏi thông tin sự kiện: lịch diễn, giá vé, địa điểm, hạng vé), " +
            "\"order_status\" (hỏi về vé/đơn hàng của chính khách), " +
            "\"faq\" (hỏi chính sách đổi/hoàn vé, quy định check-in), " +
            "\"how_to\" (hỏi cách thao tác trên nền tảng: đặt vé, thanh toán, xem lại vé), " +
            "\"recommendation\" (muốn được gợi ý sự kiện nên xem), " +
            "\"out_of_scope\" (câu hỏi không liên quan đến vé/sự kiện). " +
            "Nếu là \"event_info\" hoặc \"recommendation\", trích thêm từ khoá tên sự kiện/thể loại nếu có vào trường \"keyword\" (có thể để trống). " +
            "Bắt buộc trả về ĐÚNG định dạng JSON sau, không kèm text nào khác: {\"intent\": string, \"keyword\": string}.";

        private const string ChatSystemPrompt =
            "Bạn là trợ lý hỗ trợ khách hàng của TicketHub - nền tảng bán vé sự kiện trực tuyến. " +
            "Luôn trả lời bằng tiếng Việt, ngắn gọn, lịch sự, thân thiện. " +
            "Chỉ trả lời dựa trên dữ liệu được cung cấp trong tin nhắn hệ thống, không tự bịa thông tin sự kiện/vé không có trong dữ liệu. " +
            "Nếu dữ liệu không có thông tin khách cần, hãy nói rõ là chưa tìm thấy thay vì đoán. " +
            "Không thực hiện hoặc cam kết thực hiện bất kỳ thao tác nào thay khách (huỷ vé, hoàn tiền...), chỉ hướng dẫn khách tự thao tác trên nền tảng.";

        private const string FaqContent =
            "- Đổi/hoàn vé: khách có thể gửi yêu cầu hoàn vé trong mục \"Vé của tôi\" trước giờ diễn ra sự kiện theo chính sách của từng sự kiện; " +
            "thời gian xử lý hoàn tiền thường trong vài ngày làm việc sau khi được duyệt.\n" +
            "- Check-in: khách mang mã QR vé (trong email hoặc mục \"Vé của tôi\") đến sự kiện, nhân viên soát vé sẽ quét mã để check-in, mỗi vé chỉ check-in được một lần.\n" +
            "- Vé điện tử: vé được gửi qua email kèm mã QR ngay sau khi thanh toán thành công, khách cũng có thể xem lại trong mục \"Vé của tôi\" khi đăng nhập.";

        private const string HowToContent =
            "- Đặt vé: vào trang sự kiện → chọn suất diễn → chọn ghế/hạng vé → thêm vào giỏ → thanh toán.\n" +
            "- Thanh toán: hệ thống hỗ trợ thanh toán online qua VNPay, sau khi thanh toán thành công đơn hàng sẽ chuyển trạng thái \"Đã thanh toán\".\n" +
            "- Xem lại vé: đăng nhập → vào mục \"Vé của tôi\" để xem toàn bộ vé và đơn hàng đã mua.";

        private readonly ILlmClient _llmClient;
        private readonly ICatalogAiClient _catalogAiClient;
        private readonly IOrderingAiClient _orderingAiClient;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            ILlmClient llmClient,
            ICatalogAiClient catalogAiClient,
            IOrderingAiClient orderingAiClient,
            ICacheService cacheService,
            ILogger<ChatService> logger)
        {
            _llmClient = llmClient;
            _catalogAiClient = catalogAiClient;
            _orderingAiClient = orderingAiClient;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<(bool IsSuccess, string Message, ChatResponseDto? Data)> SendMessageAsync(
            Guid userId, Guid? sessionId, string message, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ValidatorException("Nội dung tin nhắn không được để trống.");

            Guid resolvedSessionId = sessionId ?? Guid.NewGuid();
            string cacheKey = $"chat-session:{userId}:{resolvedSessionId}";

            try
            {
                List<ChatMessageDto> history = await _cacheService.GetAsync<List<ChatMessageDto>>(cacheKey, cancellationToken)
                    ?? new List<ChatMessageDto>();

                IntentClassificationResult classification = await ClassifyIntentAsync(message, cancellationToken);

                string reply = classification.Intent switch
                {
                    "event_info" => await AnswerEventInfoAsync(message, classification.Keyword, history, cancellationToken),
                    "order_status" => await AnswerOrderStatusAsync(userId, message, history, cancellationToken),
                    "faq" => await AnswerFaqAsync(message, history, cancellationToken),
                    "how_to" => await AnswerHowToAsync(message, history, cancellationToken),
                    "recommendation" => await AnswerRecommendationAsync(message, history, cancellationToken),
                    _ => AnswerOutOfScope()
                };

                history.Add(new ChatMessageDto { Role = "user", Content = message });
                history.Add(new ChatMessageDto { Role = "assistant", Content = reply });

                if (history.Count > MaxHistoryMessages)
                    history = history.Skip(history.Count - MaxHistoryMessages).ToList();

                await _cacheService.SetAsync(cacheKey, history, SessionTtl, cancellationToken);

                return (true, "Thành công.", new ChatResponseDto { SessionId = resolvedSessionId, Reply = reply });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChatService] Failed to process chat message for user {UserId}.", userId);
                return (false, "Không thể xử lý tin nhắn lúc này. Vui lòng thử lại sau.", null);
            }
        }

        private async Task<IntentClassificationResult> ClassifyIntentAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                string rawResponse = await _llmClient.CompleteAsync(ClassifyPrompt, message, cancellationToken);
                string jsonText = ExtractJsonPayload(rawResponse);

                IntentClassificationResult? parsed = JsonSerializer.Deserialize<IntentClassificationResult>(
                    jsonText,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (parsed != null && !string.IsNullOrWhiteSpace(parsed.Intent))
                    return parsed;
            }
            catch (JsonException)
            {
            }

            return new IntentClassificationResult { Intent = "out_of_scope", Keyword = string.Empty };
        }

        private async Task<string> AnswerEventInfoAsync(
            string message, string keyword, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            SearchEventsResult searchResult = await _catalogAiClient.SearchEventsAsync(keyword, null, null, 5);

            if (!searchResult.IsSuccess || searchResult.Events.Count == 0)
            {
                var emptyPromptData = new { note = "Không tìm thấy sự kiện nào khớp với yêu cầu của khách trong hệ thống." };
                return await BuildChatReplyAsync(message, emptyPromptData, history, cancellationToken);
            }

            EventDetailResult? topDetail = await _catalogAiClient.GetEventDetailAsync(searchResult.Events[0].EventId);

            var promptData = new
            {
                matchedEvents = searchResult.Events.Select(e => new
                {
                    e.Title,
                    startAt = e.StartAt,
                    endAt = e.EndAt,
                    minPrice = e.MinPrice,
                    category = e.CategoryName,
                    location = e.ProvinceCity
                }),
                topEventDetail = topDetail is { IsSuccess: true }
                    ? new
                    {
                        topDetail.Title,
                        topDetail.Description,
                        location = $"{topDetail.VenueName}, {topDetail.AddressLine}, {topDetail.ProvinceCity}",
                        showtimes = topDetail.Showtimes.Select(st => new
                        {
                            startAt = st.StartAt,
                            endAt = st.EndAt,
                            ticketTypes = st.TicketTypes.Select(tt => new { tt.TicketTypeName, tt.Price, tt.PublishedQuota })
                        })
                    }
                    : null
            };

            return await BuildChatReplyAsync(message, promptData, history, cancellationToken);
        }

        private async Task<string> AnswerOrderStatusAsync(
            Guid userId, string message, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            MyOrdersResult ordersResult = await _orderingAiClient.GetMyOrdersAsync(userId, 5);

            if (!ordersResult.IsSuccess)
            {
                return "Hiện mình chưa tra cứu được đơn hàng của bạn (hệ thống đang gián đoạn). " +
                       "Bạn vui lòng vào mục \"Vé của tôi\" để xem trực tiếp nhé.";
            }

            var promptData = new
            {
                recentOrders = ordersResult.Orders.Select(o => new
                {
                    o.EventTitle,
                    status = o.Status,
                    showtimeStartAt = o.ShowtimeStartAt,
                    totalPrice = o.TotalPrice,
                    createdAt = o.CreatedAt
                })
            };

            return await BuildChatReplyAsync(message, promptData, history, cancellationToken);
        }

        private async Task<string> AnswerFaqAsync(string message, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            var promptData = new { faqContent = FaqContent };
            return await BuildChatReplyAsync(message, promptData, history, cancellationToken);
        }

        private async Task<string> AnswerHowToAsync(string message, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            var promptData = new { howToContent = HowToContent };
            return await BuildChatReplyAsync(message, promptData, history, cancellationToken);
        }

        private async Task<string> AnswerRecommendationAsync(string message, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            DateOnly to = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly from = to.AddDays(-29);

            CategoryTrendResult trend = await _catalogAiClient.GetCategoryTrendAsync(from, to);
            if (!trend.IsSuccess || trend.Categories.Count == 0)
            {
                var emptyPromptData = new { note = "Hiện chưa có đủ dữ liệu xu hướng để gợi ý sự kiện." };
                return await BuildChatReplyAsync(message, emptyPromptData, history, cancellationToken);
            }

            var topCategory = trend.Categories.OrderByDescending(c => c.ViewGrowthPercent).First();
            SearchEventsResult searchResult = await _catalogAiClient.SearchEventsAsync(string.Empty, topCategory.CategoryId, null, 5);

            var promptData = new
            {
                trendingCategory = topCategory.CategoryName,
                recommendedEvents = searchResult.IsSuccess
                    ? searchResult.Events.Select(e => new { e.Title, startAt = e.StartAt, minPrice = e.MinPrice, location = e.ProvinceCity })
                    : Enumerable.Empty<object>()
            };

            return await BuildChatReplyAsync(message, promptData, history, cancellationToken);
        }

        private static string AnswerOutOfScope()
        {
            return "Mình chỉ hỗ trợ các câu hỏi liên quan đến sự kiện, vé và đơn hàng trên TicketHub thôi. " +
                   "Bạn có câu hỏi nào khác về việc đặt vé/xem sự kiện không?";
        }

        private async Task<string> BuildChatReplyAsync(
            string userMessage, object contextData, List<ChatMessageDto> history, CancellationToken cancellationToken)
        {
            string contextJson = JsonSerializer.Serialize(contextData);

            List<ChatHistoryMessage> messages = history
                .TakeLast(MaxHistoryMessages)
                .Select(m => new ChatHistoryMessage { Role = m.Role, Content = m.Content })
                .ToList();

            messages.Add(new ChatHistoryMessage { Role = "system", Content = $"Dữ liệu tham khảo (JSON): {contextJson}" });
            messages.Add(new ChatHistoryMessage { Role = "user", Content = userMessage });

            return await _llmClient.ChatCompleteAsync(ChatSystemPrompt, messages, cancellationToken);
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

        private class IntentClassificationResult
        {
            [JsonPropertyName("intent")]
            public string Intent { get; set; } = "out_of_scope";

            [JsonPropertyName("keyword")]
            public string Keyword { get; set; } = string.Empty;
        }
    }
}
