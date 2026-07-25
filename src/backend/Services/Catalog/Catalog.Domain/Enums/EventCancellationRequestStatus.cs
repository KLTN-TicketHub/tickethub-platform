using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum EventCancellationRequestStatus
    {
        [Display(Name = "Chờ duyệt")]
        Pending = 1,

        [Display(Name = "Đã duyệt")]
        Approved = 2,

        [Display(Name = "Bị từ chối")]
        Rejected = 3
    }
}
