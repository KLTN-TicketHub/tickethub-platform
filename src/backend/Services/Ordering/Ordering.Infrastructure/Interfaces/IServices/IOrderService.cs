using Ordering.Common.Dtos;

namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<Guid> CheckoutAsync(CheckoutRequestDto request, Guid userId);

        Task<PendingOrderDto?> GetMyPendingOrderAsync(Guid userId, Guid showtimeId);

        Task CancelPendingOrderAsync(Guid orderId, Guid userId);
    }
}
