using System;

namespace Inventory.Infrastructure.Dtos
{
    public class TicketInventoryStateDto
    {
        public Guid ShowTimeId { get; set; }
        public Guid TicketTypeId { get; set; }
        public int Capacity { get; set; }
        public int SoldQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int LockedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
