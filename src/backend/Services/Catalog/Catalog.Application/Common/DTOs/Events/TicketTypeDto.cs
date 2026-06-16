namespace Catalog.Application.Common.DTOs.Events
{
    public class TicketTypeDto
    {
        public Guid Id { get; set; }

        public Guid? ZoneId { get; set; }

        public string TicketTypeName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int PublishedQuota { get; set; }

        public int MinQtyQuota { get; set; }
        public int MaxQtyQuota { get; set; }

        public int DisplayOrder { get; set; }

        public string Status { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
