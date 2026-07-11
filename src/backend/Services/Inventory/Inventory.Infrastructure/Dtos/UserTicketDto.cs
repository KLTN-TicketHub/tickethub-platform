using Inventory.Infrastructure.Entities;

namespace Inventory.Infrastructure.Dtos
{
    public class UserTicketDto
    {
        public Guid IssuedTicketId { get; set; }
        public Guid OrderId { get; set; }
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public string EventImage { get; set; } = string.Empty;
        public string QrCodeToken { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public DateTime ShowtimeStartAt { get; set; }
        public string? SeatName { get; set; }
        public string? RowName { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IssuedTicketStatus Status { get; set; }
    }
}
