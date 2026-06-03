using BuildingBlocks.Domain.DDD;
using Catalog.Domain.Enums;

namespace Catalog.Domain.Entities
{
    public class ZonePricing : SoftDeleteEntity, IAggregateRoot
    {
        public Event? Event { get; set; }
        public Guid EventId { get; set; }

        public Zone? Zone { get; set; }
        public Guid ZoneId { get; set; }

        public TicketType? TicketType { get; set; }
        public Guid TicketTypeId { get; set; }

        //Giá niêm yết
        public decimal ListedPrice { get; set; }

        public int PublishedQuota { get; set; }

        public CatalogStatus Status { get; set; }
    }
}
