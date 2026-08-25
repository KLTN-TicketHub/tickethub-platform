namespace Ordering.Common.Dtos.Reports
{
    public class AdminOrderListItemDto
    {
        public Guid OrderId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = default!;
        public string OrganizerName { get; set; } = default!;
        public string CustomerName { get; set; } = default!;
        public string CustomerEmail { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
