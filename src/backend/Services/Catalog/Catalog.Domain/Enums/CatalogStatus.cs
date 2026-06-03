using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum CatalogStatus
    {
        [Display(Name = "Hoạt động")]
        Active = 1,

        [Display(Name = "Ngừng hoạt động")]
        Inactive = 2,

        [Display(Name = "Đã lưu trữ")]
        Archived = 3
    }
}