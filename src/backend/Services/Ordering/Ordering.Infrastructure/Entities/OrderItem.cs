using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid? SeatId { get; set; }

        public string? SeatName { get; set; }

        public string? RowName { get; set; }

        public Guid TicketTypeId { get; set; }

        public string TicketTypeName { get; set; } = default!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Order? Order { get; set; }
    }
}
