using System.ComponentModel.DataAnnotations;

namespace Inventory.Common.Enums
{
    public enum ShowtimeSeatStatus
    {
        /// <summary>
        /// Đã đặt / Giữ chỗ (Đơn hàng đã tạo thành công, đang chờ thanh toán dài hạn hoặc đặt giữ chỗ trước)
        /// </summary>
        [Display(Name = "Đã đặt")]
        Reserved = 1,

        /// <summary>
        /// Đã bán (Đã thanh toán thành công, giao dịch hoàn tất)
        /// </summary>
        [Display(Name = "Đã bán")]
        Sold = 2,

        /// <summary>
        /// Đã hoàn tiền (Vé bị hủy và hệ thống đã hoàn trả lại tiền cho khách)
        /// </summary>
        [Display(Name = "Đã hoàn tiền")]
        Refunded = 3,

        /// <summary>
        /// Đã hủy (Khách chủ động hủy đơn, hoặc quá hạn thanh toán mà không trả tiền)
        /// </summary>
        [Display(Name = "Đã hủy")]
        Cancelled = 4
    }
}
