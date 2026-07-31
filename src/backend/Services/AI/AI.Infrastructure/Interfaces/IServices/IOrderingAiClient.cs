namespace AI.Infrastructure.Interfaces.IServices
{
    public class OrderSummaryResult
    {
        public Guid OrderId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ShowtimeStartAt { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MyOrdersResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<OrderSummaryResult> Orders { get; set; } = new();
    }

    public interface IOrderingAiClient
    {
        Task<MyOrdersResult> GetMyOrdersAsync(Guid userId, int maxResults);
    }
}
