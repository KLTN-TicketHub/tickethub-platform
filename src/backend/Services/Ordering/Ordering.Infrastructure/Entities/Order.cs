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

        public Order(Guid userId, Guid showtimeId, Guid eventId, string eventTitle, DateTime showtimeStartAt, decimal totalPrice, string paymentMethod)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ShowTimeId = showtimeId;
            EventId = eventId;
            EventTitle = eventTitle;
            ShowtimeStartAt = showtimeStartAt;
            TotalPrice = totalPrice;
            PaymentMethod = paymentMethod;
            Status = OrderStatus.Pending;
            SetCreated(userId);
        }
        public void AddOrderItem(OrderItem item)
        {
            _orderItems.Add(item);
        }
        public void MarkAsPaid()
        {
            Status = OrderStatus.Paid;
            SetUpdated(UserId);
        }
        public void CancelOrder(Guid? updatedBy = null)
        {
            Status = OrderStatus.Cancelled;
            SetUpdated(updatedBy);
        }
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
