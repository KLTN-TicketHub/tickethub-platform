using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum EventApprovalStatus
    {
        [Display(Name = "Chờ duyệt")]
        Pending = 1,

        [Display(Name = "Đã duyệt")]
        Approved = 2,

        [Display(Name = "Từ chối")]
        Rejected = 3
    }
}