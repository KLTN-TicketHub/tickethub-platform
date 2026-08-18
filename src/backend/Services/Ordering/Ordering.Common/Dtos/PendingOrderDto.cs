namespace Ordering.Common.Dtos
{
    public class PendingOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ShowtimeId { get; set; }
        public string EventTitle { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
