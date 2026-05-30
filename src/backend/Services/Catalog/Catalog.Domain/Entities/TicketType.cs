using BuildingBlocks.Domain.DDD;

namespace Catalog.Domain.Entities
{
    public class TicketType : SoftDeleteEntity, IAggregateRoot
    {
        public string TicketTypeName { get; set; }

        public string TicketTypeCode { get; set; }

        public string? Description { get; set; }

        public string Color { get; set; }

        public int DisplayOrder { get; set; }

        public string Status { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
