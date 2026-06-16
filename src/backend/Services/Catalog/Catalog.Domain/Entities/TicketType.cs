using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class TicketType : SoftDeleteEntity, IAggregateRoot
    {
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public Guid? ZoneId { get; set; }
        public Zone? Zone { get; set; }

        public string TicketTypeName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int PublishedQuota { get; set; }

        public int MinQtyQuota { get; set; }
        public int MaxQtyQuota { get; set; }

        public int DisplayOrder { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;

        public TicketType(
            string ticketTypeName,
            decimal price,
            int publishedQuota,
            int minQtyQuota,
            int maxQtyQuota,
            int displayOrder,
            string? description = null,
            Guid? zoneId = null)
        {
            TicketTypeName = ticketTypeName;
            Price = price;
            PublishedQuota = publishedQuota;
            MinQtyQuota = minQtyQuota;
            MaxQtyQuota = maxQtyQuota;
            DisplayOrder = displayOrder;
            Description = description;
            ZoneId = zoneId;
            Status = CatalogStatus.Active;
        }
    }
}
