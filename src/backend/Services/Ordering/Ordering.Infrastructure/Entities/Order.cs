using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    public class Order : SoftDeleteEntity, IAggregateRoot
    {
        public Guid UserId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerPhone { get; set; }

        public Guid ShowTimeId { get; set; }

        public Guid EventId { get; set; }

        public string EventTitle { get; set; } = default!;

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; } = default!;

        public Guid OrganizerId { get; set; }

        public string OrganizerName { get; set; } = default!;

        public string EventImage { get; set; } = default!;

        public DateTime ShowtimeStartAt { get; set; }

        public DateTime ShowtimeEndAt { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public string PaymentMethod { get; set; } = default!;

        public byte[] RowVersion { get; private set; } = default!;

        private readonly List<OrderItem> _orderItems = new List<OrderItem>();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public Order(
            Guid userId,
            string customerName,
            string customerEmail,
            string customerPhone,
            Guid showTimeId,
            Guid eventId,
            string eventTitle,
            Guid categoryId,
            string categoryName,
            Guid organizerId,
            string organizerName,
            string eventImage,
            DateTime showtimeStartAt,
            DateTime showtimeEndAt,
            decimal totalPrice,
            string paymentMethod)
        {
            UserId = userId;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            ShowTimeId = showTimeId;
            EventId = eventId;
            EventTitle = eventTitle;
            CategoryId = categoryId;
            CategoryName = categoryName;
            OrganizerId = organizerId;
            OrganizerName = organizerName;
            EventImage = eventImage;
            ShowtimeStartAt = showtimeStartAt;
            ShowtimeEndAt = showtimeEndAt;
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
