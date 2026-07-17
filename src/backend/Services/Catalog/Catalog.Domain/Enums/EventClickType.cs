using System.ComponentModel.DataAnnotations;

namespace Catalog.Domain.Enums
{
    public enum EventClickType
    {
        [Display(Name = "Xem chi tiết")]
        ViewDetail = 1,

        [Display(Name = "Có ý định mua vé")]
        PurchaseIntent = 2
    }
}
