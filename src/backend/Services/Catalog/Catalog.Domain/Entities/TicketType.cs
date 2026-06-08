using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class TicketType : SoftDeleteEntity, IAggregateRoot
    {
        public Event? Event { get; set; }
        public Guid EventId { get; set; }

        public Zone? Zone { get; set; }
        public Guid? ZoneId { get; set; }

        public string TicketTypeName { get; set; }

        public string TicketTypeCode { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int PublishedQuota { get; set; }

        public int MinQtyQuota { get; set; }
        public int MaxQtyQuota { get; set; }

        public string Color { get; set; }

        public int DisplayOrder { get; set; }

        public CatalogStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
