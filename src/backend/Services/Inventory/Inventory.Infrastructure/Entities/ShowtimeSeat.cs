using System;
using Inventory.Common.Enums;

namespace Inventory.Infrastructure.Entities
{
    public class ShowtimeSeat
    {
        public Guid Id { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
        public Guid? OrderId { get; set; }
        public string? UserId { get; set; }
        
        // Denormalized fields for quick ticket rendering & check-in scanning
        public decimal Price { get; set; }
        public string RowName { get; set; } = default!;
        public int SeatNumber { get; set; }
        
        // Seating transaction status
        public ShowtimeSeatStatus Status { get; set; } = ShowtimeSeatStatus.Sold;
        
        // Fields for check-in gates
        public bool IsCheckedIn { get; set; } = false;
        public DateTime? CheckedInAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
