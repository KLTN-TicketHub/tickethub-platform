using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum EventStatus
    {
        [Display(Name = "Chờ duyệt")]
        PendingApproval = 1,

        [Display(Name = "Đã xuất bản")]
        Published = 2,

        [Display(Name = "Đã qua")]
        Archived = 3
    }
}