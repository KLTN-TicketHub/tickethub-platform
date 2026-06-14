namespace Catalog.Application.Features.Events.Requests
{
    public class CreateTicketTypeRequest
    {
        public Guid? ZoneId { get; set; }

        public string TicketTypeName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? PublishedQuota { get; set; }

        public int MinQtyQuota { get; set; }
        public int MaxQtyQuota { get; set; }

        public int DisplayOrder { get; set; }
    }
}
