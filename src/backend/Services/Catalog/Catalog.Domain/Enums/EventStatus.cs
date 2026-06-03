using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum EventStatus
    {
        [Display(Name = "Bản nháp")]
        Draft = 1,

        [Display(Name = "Chờ duyệt")]
        PendingApproval = 2,

        [Display(Name = "Đã xuất bản")]
        Published = 3,

        [Display(Name = "Đã hủy")]
        Cancelled = 4,

        [Display(Name = "Đã lưu trữ")]
        Archived = 5
    }
}