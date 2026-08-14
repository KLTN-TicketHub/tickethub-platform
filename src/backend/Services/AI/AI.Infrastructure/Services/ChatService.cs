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
        "Bạn là bộ phân loại ý định (intent classifier) cho chatbot hỗ trợ khách hàng của TicketHub, " +
        "một nền tảng bán vé sự kiện trực tuyến. " +
        "\n\n" +

        "NHIỆM VỤ:" +
        "\n" +
        "Đọc DUY NHẤT tin nhắn của khách hàng và phân loại nó vào ĐÚNG MỘT intent. " +
        "Mục tiêu của bạn là xác định ý định thực tế của khách hàng, không phải suy đoán " +

        "khách hàng đang nói đến sự kiện hoặc đối tượng cụ thể nào." +
        "\n\n" +

        "CÁC INTENT:" +
        "\n" + "\"event_info\": " +

        "Bao gồm thông tin như tên sự kiện, lịch diễn, ngày giờ, địa điểm, giá vé, " +
        "Khách hàng hỏi hoặc tìm kiếm thông tin về sự kiện. " +

        "loại vé, hạng vé, ghế hoặc các thông tin thuộc về một sự kiện. " +
        "\n\n" +
        "Khách hàng hỏi về vé, đơn hàng hoặc giao dịch của chính họ. " +
        "\"order_status\": " +

        "Chỉ sử dụng intent này khi nội dung thể hiện mối quan hệ với vé hoặc đơn hàng của khách hàng." +
        "\n\n" +
        "Khách hàng hỏi về chính sách, quy định hoặc điều kiện chung của TicketHub hoặc việc sử dụng vé. " +
        "\"faq\": " +

        "Bao gồm chính sách đổi vé, hoàn vé, check-in và các quy định tương tự." +
        "\n\n" +
        "Khách hàng hỏi cách thực hiện một thao tác trên nền tảng. " +
        "\"how_to\": " +

        "Bao gồm cách đặt vé, thanh toán, xem vé, xem đơn hàng hoặc sử dụng các chức năng của TicketHub." +
        "\n\n" +
        "Khách hàng thể hiện rõ mong muốn được hệ thống gợi ý, đề xuất hoặc lựa chọn sự kiện phù hợp. " +
        "\"recommendation\": " +
        "Không được tự suy ra recommendation chỉ vì khách hàng đề cập đến một chủ đề, thể loại, " +
        "Chỉ sử dụng intent này khi có dấu hiệu rõ ràng rằng khách hàng đang yêu cầu recommendation. " +

        "môn thể thao, nghệ sĩ, địa điểm hoặc loại sự kiện." +
        "\n\n" + "\"out_of_scope\": " +


        "Nội dung không liên quan đến việc tìm hiểu, mua, sử dụng hoặc quản lý vé/sự kiện trên TicketHub." +
        "\n\n" + "NGUYÊN TẮC QUAN TRỌNG NHẤT:" +

        "1. CHỈ dựa trên nội dung thực tế trong tin nhắn của khách hàng." +
        "\n" +
        "2. KHÔNG được sử dụng kiến thức bên ngoài để bổ sung hoặc thay đổi ý nghĩa của tin nhắn." +
        "\n" +
        "3. KHÔNG được tự suy đoán khách hàng đang nói đến một sự kiện cụ thể nếu họ không nói rõ." +
        "\n" +
        "4. KHÔNG được tự suy đoán tên sự kiện, giải đấu, chương trình, nghệ sĩ, đội bóng, địa điểm, " +
        "\n" +
        "\n" +
        "thương hiệu hoặc bất kỳ thực thể cụ thể nào." +
        "\n" +
        "5. KHÔNG được biến một từ khóa hoặc chủ đề chung thành một thực thể cụ thể." +
        "mà khách hàng chưa nói." +
        "6. KHÔNG được dùng kiến thức phổ biến trên Internet hoặc kiến thức đã biết để hoàn thiện câu hỏi " +
        "7. Nếu thông tin trong tin nhắn không đủ để xác định một thực thể cụ thể, hãy giữ nguyên mức độ " +
        "\n" +
        "\n" +
        "khái quát mà khách hàng đã sử dụng." +

        "8. Không được tạo ra thông tin mới trong quá trình phân loại." +
        "\n\n" +
        "\n" +
        "XỬ LÝ KEYWORD:" +
        "được lấy từ chính tin nhắn của khách hàng." +
        "Nếu intent là event_info hoặc recommendation, trường keyword phải chứa từ khóa liên quan " +
        "Keyword phải phản ánh đúng những gì khách hàng thực sự đề cập." +
        "\n" +
        "Không được tự thêm thông tin vào keyword." +
        "\n" +
        "Không được mở rộng keyword thành một thực thể cụ thể hơn." +
        "\n" +
        "Không được thay thế keyword bằng một thực thể mà model cho rằng có liên quan." +
        "\n" +
        "Nếu khách hàng không cung cấp từ khóa đủ rõ ràng thì để keyword là chuỗi rỗng." +
        "\n" +
        "Keyword có thể là tên sự kiện, tên nghệ sĩ, thể loại, chủ đề hoặc cụm từ tìm kiếm " +
        "\n" +

        "nếu những thông tin đó thực sự xuất hiện trong tin nhắn." +
        "\n\n" + "PHÂN BIỆT GIỮA EVENT_INFO VÀ RECOMMENDATION:" +

        "event_info dùng khi khách hàng muốn tìm hiểu hoặc tìm kiếm sự kiện/thông tin sự kiện." +
        "\n" +
        "recommendation chỉ dùng khi khách hàng thực sự yêu cầu hệ thống đưa ra đề xuất hoặc gợi ý." +
        "\n" +
        "Không được suy luận mong muốn recommendation chỉ từ việc khách hàng đề cập đến một chủ đề " +
        "\n" +

        "hoặc loại sự kiện." +
        "\n\n" +
        "\n" +
        "PHÂN BIỆT GIỮA FAQ VÀ HOW_TO:" +
        "\n" +
        "faq dùng cho câu hỏi về chính sách, quy định, điều kiện hoặc quyền lợi." +

        "how_to dùng cho câu hỏi về cách thực hiện một thao tác trên hệ thống." +
        "\n\n" +
        "\n" +
        "PHÂN BIỆT GIỮA EVENT_INFO VÀ ORDER_STATUS:" +
        "\n" +
        "event_info liên quan đến thông tin của sự kiện nói chung." +

        "order_status liên quan đến vé, đơn hàng hoặc giao dịch của chính khách hàng." +
        "\n\n" +
        "\n" +
        "QUY TẮC KHI CÂU HỎI MƠ HỒ:" +
        "\n" +
        "Nếu câu hỏi ngắn, thiếu ngữ cảnh hoặc chỉ chứa một chủ đề, không được tự tạo thêm ngữ cảnh." +
        "\n" +
        "Hãy phân loại dựa trên ý nghĩa trực tiếp và rõ ràng nhất của nội dung đã nhận được." +
        "\n" +
        "Không được cố gắng đoán xem khách hàng 'có thể' đang muốn hỏi điều gì." +

        "Không được sử dụng kiến thức bên ngoài để giải quyết sự mơ hồ." +
        "\n\n" +
        "\n" +
        "ĐỊNH DẠNG OUTPUT:" +
        "\n" +
        "Bắt buộc trả về ĐÚNG một JSON object." +
        "\n" +
        "Không được trả về markdown." +
        "\n" +
        "Không được giải thích." +
        "\n" +
        "Không được thêm text trước hoặc sau JSON." +
        "\n" +
        "JSON phải có chính xác hai field: \"intent\" và \"keyword\"." +
        "\"event_info\", \"order_status\", \"faq\", \"how_to\", \"recommendation\", \"out_of_scope\"." +
        "\"intent\" chỉ được nhận một trong các giá trị: " +
        "\"keyword\" luôn phải là string." +
        "\n" +
        "Nếu không có keyword phù hợp, trả về chuỗi rỗng.";

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
