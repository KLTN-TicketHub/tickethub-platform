using BuildingBlocks.Domain.DDD;

namespace Inventory.Infrastructure.Entities
{
    public class ShowtimeTicketInventory : BaseEntity, IAggregateRoot
    {
        public Guid ShowTimeId { get; set; }

        public Guid TicketTypeId { get; set; }

        public int Capacity { get; set; }

        public int SoldQuantity { get; set; }

        public int ReservedQuantity { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
