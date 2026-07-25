using BuildingBlocks.Domain.DDD;

namespace Inventory.Infrastructure.Entities
{
    public class IssuedTicket : BaseEntity, IAggregateRoot
    {
        public Guid OrderId { get; set; }

        public Guid ShowTimeId { get; set; }

        public Guid EventId { get; set; }

        public string EventTitle { get; set; } = string.Empty;

        public string OrganizerName { get; set; } = string.Empty;

        public string EventImage { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        public Guid? SeatId { get; set; }

        public string? SeatName { get; set; }

        public string? RowName { get; set; }

        public Guid TicketTypeId { get; set; }

        public string TicketTypeName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string QrCodeToken { get; set; } = string.Empty;

        public string QrCodeBase64 { get; set; } = string.Empty;

        public DateTime ShowtimeStartAt { get; set; }

        public IssuedTicketStatus Status { get; set; }

        public void Cancel()
        {
            if (Status == IssuedTicketStatus.Valid)
                Status = IssuedTicketStatus.Cancelled;
        }
    }

    public enum IssuedTicketStatus
    {
        Valid = 1,
        Used = 2,
        Cancelled = 3
    }
}
