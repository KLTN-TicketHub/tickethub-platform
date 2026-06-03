using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum SeatLayoutStatus
    {
        [Display(Name = "Có sẵn")]
        Available = 1,

        [Display(Name = "Không có sẵn")]
        Unavailable = 2
    }
}