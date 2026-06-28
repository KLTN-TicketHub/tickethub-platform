using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    public class Order : SoftDeleteEntity, IAggregateRoot
    {
        public Guid UserId { get; set; }

        public Guid ShowTimeId { get; set; }

        public Guid EventId { get; set; }

        public string EventTitle { get; set; } = default!;

        public DateTime ShowtimeStartAt { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public string PaymentMethod { get; set; } = default!;

        public byte[] RowVersion { get; private set; } = default!;

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    }
    public enum OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Completed = 3,
        Cancelled = 4,
        Refunded = 5
    }
}
