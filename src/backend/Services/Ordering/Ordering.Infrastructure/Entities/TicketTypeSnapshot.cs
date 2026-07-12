using BuildingBlocks.Domain.DDD;

namespace Ordering.Infrastructure.Entities
{
    public class TicketTypeSnapshot : BaseEntity
    {
        public Guid ShowtimeSnapshotId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public bool IsReservingSeat { get; set; }
        public ShowtimeSnapshot ShowtimeSnapshot { get; set; } = default!;

        public TicketTypeSnapshot(Guid showtimeSnapshotId, Guid ticketTypeId, string ticketTypeName, decimal price, int capacity, bool isReservingSeat)
        {
            ShowtimeSnapshotId = showtimeSnapshotId;
            TicketTypeId = ticketTypeId;
            TicketTypeName = ticketTypeName;
            Price = price;
            Capacity = capacity;
            IsReservingSeat = isReservingSeat;
        }
    }
}
