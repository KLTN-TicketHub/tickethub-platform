using BuildingBlocks.Domain.DDD;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Infrastructure.Entities
{
    public class ShowtimeSeat : BaseEntity, IAggregateRoot
    {
        public Guid ShowTimeId { get; set; }

        public Guid SeatId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid UserId { get; set; }

        public decimal Price { get; set; }

        public string Row { get; set; }

        public string SeatName { get; set; }

        public SeatStatus SeatStatus { get; set; }
    }

    public enum SeatStatus
    {
        [Display(Name = "Đã bán")]
        Sold = 1,

        [Display(Name = "Đã đặt")]
        Reserved = 2,

        [Display(Name = "Đã hủy")]
        Cancelled = 3
    }
}
