using Ordering.Common.Dtos;

namespace Ordering.Infrastructure.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<(bool IsSuccess, Guid OrderId, string Message)> CheckoutAsync(CheckoutRequestDto request, Guid userId);
    }
}
